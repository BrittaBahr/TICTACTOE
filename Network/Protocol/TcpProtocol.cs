using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Network.Protocol.Events;
using SharedLibrary;
using TicTacToeGameLogic;

namespace Network.Protocol
{
    public class TcpProtocol
    {
        private static readonly byte[] Header = new byte[] { 0x42, 0x53, 0x50 };

        private readonly Dictionary<byte, Action<ParseObject>> parserDict;

        public TcpProtocol()
        {
            this.parserDict = new Dictionary<byte, Action<ParseObject>>()
            {
                [1] = this.ParseGameTurnReceivedMessage,
                [2] = this.ParseAcknowledgeGameTurn,
                [3] = this.ParseGameRequestReceivedMessage,
                [4] = this.ParseAcknowledgeGameRequestReceived,
                [5] = this.ParseReplayRequestReceivedMessage
            };
        }

        public event EventHandler<ProtocolGameTurnReceivedEventArgs>? GameTurnReceived;

        public event EventHandler<ProtocolAcknowledgeGameTurnEventArgs>? AcknowledgeGameTurn;

        public event EventHandler<ProtocolGameRequestReceivedEventArgs>? GameRequestReceived;

        public event EventHandler<ProtocolAcknowledgeGameRequestEventArgs>? AcknowledgeGameRequest;

        public event EventHandler<ProtocolReplayRequestReceivedEventsArgs>? ReplayRequestReceived;

        public byte[] ParseMessages(byte[] receivedBuffer)
        {
            if (receivedBuffer == null)
            {
                throw new ArgumentNullException(nameof(receivedBuffer));
            }

            ParseObject parseObject = new ParseObject(receivedBuffer);

            // -5 = reading an integer (the message length) + reading a byte (the identifier)
            while (parseObject.Index < receivedBuffer.Length - Header.Length - 5)
            {
                bool validHeader = true;

                // checks the header
                for (int i = 0; i < Header.Length; i++)
                {
                    if (parseObject.ReceivedBuffer[parseObject.Index + i] != Header[i])
                    {
                        validHeader = false;
                        break;
                    }
                }

                if (!validHeader)
                {
                    parseObject.Index++;
                    continue;
                }

                int oldIndex = parseObject.Index;
                parseObject.Index += Header.Length;
                int messageLength = this.ReadInteger(parseObject);

                try
                {
                    if (!this.AssertCheckSum(parseObject, messageLength))
                    {
                        parseObject.Index += messageLength;
                        parseObject.Index += 4;
                        break;
                    }
                }
                catch (MessageIncompleteException)
                {
                    parseObject.Index = oldIndex;
                    break;
                }

                byte identifier = parseObject.ReceivedBuffer[parseObject.Index];

                if (!this.parserDict.TryGetValue(identifier, out Action<ParseObject>? parser))
                {
                    parseObject.Index++;
                    continue;
                }

                try
                {
                    parser(parseObject);
                }
                catch (MessageIncompleteException)
                {
                    break;
                }
            }

            return receivedBuffer.Skip(parseObject.Index).ToArray(); // skips all bytes, which were parsed in a correct form
        }

        public byte[] ConvertGameTurn(Position position)
        {
            List<byte> message = new List<byte>() { 1 };
            message.AddRange(BitConverter.GetBytes(position.XPosition));
            message.AddRange(BitConverter.GetBytes(position.YPosition));

            return this.AddMessageEnvelope(message);
        }

        public byte[] ConvertAcknowledgeGameTurn()
        {
            return this.AddMessageEnvelope(new List<byte>() { 2 });
        }

        public byte[] ConvertGameRequest(string nickname, IPAddress iPAddress)
        {
            List<byte> message = new List<byte>() { 3 };
            // nickname
            byte[] nameBytes = Encoding.UTF8.GetBytes(nickname);
            byte[] nameLengthBytes = BitConverter.GetBytes(nameBytes.Length);
            message.AddRange(nameLengthBytes);
            message.AddRange(nameBytes);

            // ipAddress
            byte[] ipAddressBytes = iPAddress.GetAddressBytes();
            byte[] ipAddressLengthBytes = BitConverter.GetBytes(ipAddressBytes.Length);
            message.AddRange(ipAddressLengthBytes);
            message.AddRange(ipAddressBytes);

            return this.AddMessageEnvelope(message);
        }

        public byte[] ConvertAcknowledgeGameRequest()
        {
            return this.AddMessageEnvelope(new List<byte>() { 4 });
        }

        public byte[] ConvertReplayRequest(string nickname, IPAddress iPAddress)
        {
            return this.AddMessageEnvelope(new List<byte>() { 5 });
        }

        protected virtual void OnGameTurnReceived(Position position)
        {
            this.GameTurnReceived?.Invoke(this, new ProtocolGameTurnReceivedEventArgs(position));
        }

        protected virtual void OnAcknowledgeGameTurn()
        {
            this.AcknowledgeGameTurn?.Invoke(this, new ProtocolAcknowledgeGameTurnEventArgs());
        }

        protected virtual void OnGameRequestReceived(string nickname, IPAddress iPAddress)
        {
            this.GameRequestReceived?.Invoke(this, new ProtocolGameRequestReceivedEventArgs(nickname, iPAddress));
        }

        protected virtual void OnAcknowledgeGameRequest()
        {
            this.AcknowledgeGameRequest?.Invoke(this, new ProtocolAcknowledgeGameRequestEventArgs());
        }

        protected virtual void OnReplayRequestReceived()
        {
            this.ReplayRequestReceived?.Invoke(this, new ProtocolReplayRequestReceivedEventsArgs());
        }

        private bool AssertCheckSum(ParseObject parseObject, int messageLength)
        {
            byte[] neededByteArray = parseObject.ReceivedBuffer.Skip(parseObject.Index).Take(messageLength).ToArray();

            if (neededByteArray.Length != messageLength)
            {
                throw new MessageIncompleteException();
            }

            int calculateCheckSum = neededByteArray[0];

            for (int i = 1; i < neededByteArray.Length; i++)
            {
                calculateCheckSum += neededByteArray[i] * i;
            }

            int oldIndex = parseObject.Index;
            parseObject.Index += messageLength;
            int checkSum = this.ReadInteger(parseObject);
            parseObject.Index = oldIndex;

            return checkSum == calculateCheckSum;
        }

        private byte[] AddMessageEnvelope(List<byte> messageBytes)
        {
            byte[] dataLengthBytes = BitConverter.GetBytes(messageBytes.Count);
            int checkSum = messageBytes[0];

            for (int i = 1; i < messageBytes.Count; i++)
            {
                checkSum += messageBytes[i] * i;
            }

            byte[] checkSumBytes = BitConverter.GetBytes(checkSum);

            return Header.Concat(dataLengthBytes).Concat(messageBytes).Concat(checkSumBytes).ToArray();
        }

        private void ParseGameTurnReceivedMessage(ParseObject parseObject)
        {
            parseObject.Index++;
            this.OnGameTurnReceived(this.ReadPosition(parseObject));
        }

        private void ParseAcknowledgeGameTurn(ParseObject parseObject)
        {
            parseObject.Index++;
            this.OnAcknowledgeGameTurn();
        }

        private void ParseGameRequestReceivedMessage(ParseObject parseObject)
        {
            parseObject.Index++;
            int nicknameLenght = this.ReadInteger(parseObject);
            string nickname = this.ReadString(parseObject, nicknameLenght);
            int ipAddressLength = this.ReadInteger(parseObject);
            IPAddress ipAddress = this.ReadIPAddress(parseObject, ipAddressLength);
            this.OnGameRequestReceived(nickname, ipAddress);
        }

        private void ParseAcknowledgeGameRequestReceived(ParseObject parseObject)
        {
            parseObject.Index++;
            this.OnAcknowledgeGameRequest();
        }

        private void ParseReplayRequestReceivedMessage(ParseObject parseObject)
        {
            parseObject.Index++;
            this.OnReplayRequestReceived();
        }

        private int ReadInteger(ParseObject parseObject)
        {
            if (parseObject.Index > parseObject.ReceivedBuffer.Length - 4)
            {
                throw new MessageIncompleteException();
            }

            int result = BitConverter.ToInt32(parseObject.ReceivedBuffer, parseObject.Index);
            parseObject.Index += 4;

            return result;
        }

        private long ReadLong(ParseObject parseObject)
        {
            if (parseObject.Index > parseObject.ReceivedBuffer.Length - 8)
            {
                throw new MessageIncompleteException();
            }

            long result = BitConverter.ToInt64(parseObject.ReceivedBuffer, parseObject.Index);
            parseObject.Index += 8;

            return result;
        }

        private string ReadString(ParseObject parseObject, int stringLength)
        {
            if (parseObject.Index > parseObject.ReceivedBuffer.Length - stringLength)
            {
                throw new MessageIncompleteException();
            }

            string result = Encoding.UTF8.GetString(parseObject.ReceivedBuffer, parseObject.Index, stringLength);
            parseObject.Index += stringLength;

            return result;
        }

        private byte ReadByte(ParseObject parseObject)
        {
            if (parseObject.Index > parseObject.ReceivedBuffer.Length - 1)
            {
                throw new MessageIncompleteException();
            }

            byte result = parseObject.ReceivedBuffer[parseObject.Index];
            parseObject.Index++;

            return result;
        }

        private bool ReadBool(ParseObject parseObject)
        {
            if (parseObject.Index > parseObject.ReceivedBuffer.Length - 1)
            {
                throw new MessageIncompleteException();
            }

            bool result = BitConverter.ToBoolean(parseObject.ReceivedBuffer, parseObject.Index);
            parseObject.Index++;

            return result;
        }

        private Position ReadPosition(ParseObject parseObject)
        {
            if (parseObject.Index > parseObject.ReceivedBuffer.Length - 8)
            {
                throw new MessageIncompleteException();
            }

            int xPosition = this.ReadInteger(parseObject);
            int yPosition = this.ReadInteger(parseObject);

            return new Position(xPosition, yPosition);
        }

        private IPAddress ReadIPAddress(ParseObject parseObject, int ipAddressLength)
        {
            if (parseObject.Index > parseObject.ReceivedBuffer.Length - ipAddressLength)
            {
                throw new MessageIncompleteException();
            }

            IPAddress result = new IPAddress(parseObject.ReceivedBuffer.Skip(parseObject.Index).Take(ipAddressLength).ToArray());
            parseObject.Index += ipAddressLength;

            return result;
        }
    }
}

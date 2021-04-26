//-----------------------------------------------------------------------
// <copyright file="Command.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Svenja Bahr, Britta Bahr</author>
// <summary>This class implements the ICommand interface.</summary>
//-----------------------------------------------------------------------

namespace WpfClient
{
    using System;
    using System.Windows.Input;

    /// <summary>This class implements the ICommand interface.</summary>
    public class Command : ICommand
    {
        /// <summary>The action of the command.</summary>
        private Action<object> action;

        /// <summary>Initializes a new instance of the <see cref="Command"/> class.</summary>
        /// <param name="action">The action of the command.</param>
        public Command(Action<object> action)
        {
            this.action = action;
        }

        /// <summary>Needed if changes appear and they affects the command.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>  If the command can be executed or not.</summary>
        /// <param name="parameter">The parameters of the commands.</param>
        /// <returns>
        ///   True if the command can be executed.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>  Executes the command and the action invokes.</summary>
        /// <param name="parameter">  The parameters of the command.</param>
        public void Execute(object parameter)
        {
            this.action.Invoke(parameter);
        }
    }
}

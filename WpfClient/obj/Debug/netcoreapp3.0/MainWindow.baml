   M S B A M L   `   `   `  ����D  @WpfClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:WpfClient.ConvertersWpfClient.Converters  5   	   %clr-namespace:WpfClient	WpfClient  5   	       WpfClient.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   8	converter"clr-namespace:WpfClient.Converters     5   	   )localclr-namespace:WpfClient     5   	     � Title$  	TicTacToe��5	   	   $	��450��6   $	��800��6(    � 	Resources 5
      <   6WpfClient.Converters.AlphaRedGreenBlueToColorConverter   ColorConverter4   .WpfClient.Converters.NullToVisibilityConverter  NullVisConverter��%:   &	        &	        5   
   66     5   
   6.   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��0��6    $��Center=�6-   y�6Q   �� *
WhosTurn+ �� 	Converter# [ ��5      �� 5       �� Color 6"   �� *ActivePlayerVM.ARGB+# [  .��5      �� $��Center=�6    �� Orientation$ 
Horizontal=�5      .+�5       �� $��24(�6   $��0 0 5 0q�6*   Current Player:6L   �� 5!      $��24(�6   ��6*   �� *
WhosTurn+5"      5#      �� 5&   
   $��0��6   $��1��6'   ��64   �� *	FieldVM+��5'      �� 5(      .R�5)      A�  A� Columns 6"   �� *FieldVM.Count+5*      ��5,      �� 5-      .R�5.      �� ��6#   �� ��5/      �� 50      .R�51   "   A�  A� Rows 6.   �� *Count+52      ��54      �� 55      .R�56   "   �� ��57   &   �� 58   *   	 �� 
TargetType"	 ��60   
 �� Triggers
 59   .   �� 5:   2    �� Binding 6>   �� *Winner+ �� Value 6Y   Z� .a�5;   6   ��  �� Property � �� �� Value 6P   �� *CellCommand+5<   3   �� 5=   2    6>   �� *	IsEnabled+	 False6\   .a�5>   6   ��  � �� 6P   Z� 5?   3   5A   +   ��5C   &   �� 5D   *    6:   �� *ARGB+# [  5F   #   5G      5I      5J      5L      �� 5O   
   $��0��6   $��2��6    $��Center=�6-   y�6Q   �� *Winner+# [ ��5P      �� 5Q       6"   �� *Winner.ARGB+# [  .��5T      �� $��Center=�6   $ 
Horizontal=�5U      .+�5W      �� $��24(�6   $��0 0 5 0q�6*   	Winner:6D   �� 5X      $��24(�6   ��6*   �� *	WhoWins+5Y      5Z      5[      5\      
   M S B A M L   `   `   `  ���� I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null[2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages$WDG.Wallet.Win.ViewModels.ShellPages  Q-clr-namespace:WDG.Wallet.Win.Views.ShellPagesWDG.Wallet.Win.Views.ShellPages  O,clr-namespace:WDG.Wallet.Win.ValidationRulesWDG.Wallet.Win.ValidationRules  1    +WDG.Wallet.Win.Views.ShellPages.OpenUrlPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Avm2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages     ?local-clr-namespace:WDG.Wallet.Win.Views.ShellPages     >rules,clr-namespace:WDG.Wallet.Win.ValidationRules     $	��270��$	��550��  � DataContext  ;   5WDG.Wallet.Win.ViewModels.ShellPages.OpenUrlViewModel   � 	Resources ��%    .��� 
������� � RowDefinitions �� $��40 ��� $��* �.+�2   ,WDG.Wallet.Win.Views.ShellPages.TitleControl     page_OpenUrl   Title# �   �� $��1�� �� CornerRadius$ 0 0 1 1��._��  � ColumnDefinitions �� $
��Auto ���  �� $��* ��� $
��Auto ��� $��40 ��� $��* �.+��� $	��URI��$��1��$��Center=�$��Right=�$��40 10 20 10q��� -    ��Name$ 
addressBox��$��1��  WarkTextBoxStyle#��[   tb_tooltip_inputUrl
 � Tag# �  $��Center=�$��1��$��34��$	��340��$��20 10q�$
��Left=�.������  �� Path$	 Url�	 �� Mode$	 TwoWay=�
 �� UpdateSourceTrigger$
 PropertyChanged=�.���� $��2��$��1��y��� *
IsUrlError+ ! _booleanToVisibilityConverter �� 	Converter# [ .+��� $��14��$��14��$��20 10q�$!��../../images/page/error.png����   Error_Address#���  
���  ��$��Center=�$
��Left=�$��40 10q��� $��3��$��2��$��Center=�$��Center=�$��0q�$��80��
�������$��32�� 
 btn_OK#��[ ���#3B8EFF���� *	OkCommand+��`�   _validationConverter `� 	Converter# [ 1���  �� ElementName$ 
addressBox��
   M S B A M L   `   `   `  ����I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:WDG.Wallet.Win.Views.ShellPagesWDG.Wallet.Win.Views.ShellPages  5      [2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages$WDG.Wallet.Win.ViewModels.ShellPages  5      M+clr-namespace:WDG.Wallet.Win.Views.ControlsWDG.Wallet.Win.Views.Controls  5      O,clr-namespace:WDG.Wallet.Win.ValidationRulesWDG.Wallet.Win.ValidationRules  5	      8    2WDG.Wallet.Win.Views.ShellPages.InputWalletPwdPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:WDG.Wallet.Win.Views.ShellPages     5      Avm2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages     5      ?control+clr-namespace:WDG.Wallet.Win.Views.Controls     5      Dvalidations,clr-namespace:WDG.Wallet.Win.ValidationRules     5	      c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5
      $	��400��5      $	��550��6     N� Title$  InputWalletPwd��5       � DataContext 5      B   <WDG.Wallet.Win.ViewModels.ShellPages.InputWalletPwdViewModel  5   
   .��5      � 
������� � ColumnDefinitions 5   
   �� 5      $	��120 �6   �� 5       � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��60 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   2   ,WDG.Wallet.Win.Views.ShellPages.TitleControl     InputWalletPwdPage_title   Title# �   6   $��2��6P   �� 5!   
   -   5         ��Name$ SendTip��5!      $y�	Collapsed=�6%   $��1��6<   $��1��6L   ���#666666$��15 35 20 10q�6n     InputWalletPwdPage_tooltip#���  6�   �� 5#   
     InputWalletPwdPage_inputPwd#���  6   $��2��6I   $��Right=�6V   $��0 0 0 0q�6r   $��Center=�6�   0   *WDG.Wallet.Win.Views.Controls.UserPassWord  5%   
   -   5          Name$ pwdBox��5%       $	��340��60   $��34��6<   $��15 10q�6H   $��1��6W   $��2��6g   $
��Left=�5'       $��Center=�6;      IsShowPassword 5&       �� *	IsChecked+ �� ElementName$ ck_isShowPwd��	   Password	 5(      �� 5)      
 �� Path$
 Password�6    �� Mode$ TwoWay=�6*    �� UpdateSourceTrigger$ PropertyChanged=�68    �� ValidationRules 5*      :   4WDG.Wallet.Win.ValidationRules.NotNullValidationRule  5+      1   +WDG.Wallet.Win.ValidationRules.PasswordRule  5,      5.      50      �� 52   
   -   5         ��Name$ ck_isShowPwd��52      $��15 10q�6)   $��1��68   $��3��6H     InputWalletPwdPage_showPwd#���  6U   �� 54   
   $��2��55      $��4��6%   $��32��62   $��80��6>    
 btn_OK#���  6I   $��Bottom=�6l   $
��Left=�6�   $��185 0 0 116q�6�   ��54      �� *	OkCommand+��56      `� 57        _validationConverter `� 	Converter# [ 6   1�58      �� 59      $ pwdBox��6"   5;      {�5=      `� 5>        _boolsToBoolMultiConverter# [ 6   1�5?      �� 5@      $ pwdBox��6"   $
 (Validation.Errors)�67    ' #_listCountMoreThanToEnableConverter �� 	Converter# [ 6R    �� ConverterParameter$ 1��6�   �� 5A      $ pwdBox��6"   $
 Password�67     _isNullConverter# [ 6G   $ reverse��6u   5C      5E      �� 5G   
   $��2��63   $��4��6G   $��32��6T   $��80��6`    	 
btn_Cancel#��� 	 6k   $��Bottom=�6�   $��Right=�6�   $��0 0 185 116q�6�   ��6   �� *CancelCommand+6�   5H      5I      
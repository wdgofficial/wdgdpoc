   M S B A M L   `   `   `  ���� I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null[2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages$WDG.Wallet.Win.ViewModels.ShellPages  Q-clr-namespace:WDG.Wallet.Win.Views.ShellPagesWDG.Wallet.Win.Views.ShellPages  M+clr-namespace:WDG.Wallet.Win.Views.ControlsWDG.Wallet.Win.Views.Controls  O,clr-namespace:WDG.Wallet.Win.ValidationRulesWDG.Wallet.Win.ValidationRules  9    3WDG.Wallet.Win.Views.ShellPages.PasswordSettingPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Avm2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages     ?local-clr-namespace:WDG.Wallet.Win.Views.ShellPages     ?control+clr-namespace:WDG.Wallet.Win.Views.Controls     Dvalidations,clr-namespace:WDG.Wallet.Win.ValidationRules     $	��400��$
��Auto��$	��550��  � DataContext  C   =WDG.Wallet.Win.ViewModels.ShellPages.PasswordSettingViewModel  .��� 
������� � RowDefinitions �� $��40 ��� $��* �.+�2   ,WDG.Wallet.Win.Views.ShellPages.TitleControl     Title �� *PassWordData.PageTitle+�� $��1�� �� CornerRadius$ 0 0 1 1��._��  � ColumnDefinitions �� $
��Auto ���  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��1��$��
20 50 0 10q�y��� *DisplayControlNum+ $   _intCompareToVisibilityConverter �� 	Converter# [   �� ConverterParameter$ 1��.+���   page_pwdSetting_tooltip1#���  $
��Wrap=���   page_pwdSetting_tooltip2#���  $��1��$
��Wrap=�$��0 4 0 0q��� $��1��$��
20 50 0 10q�y��� *DisplayControlNum+# [  $ 0��.+���   page_changePwd_caption1#���  $
��Wrap=���   page_changePwd_oldPwd#���  $��1��$��Center=�$
��Left=�$��40 10 20 10q�y��� *DisplayControlNum+# [  $ 0��� $��1��$��1��$��20 10q�y��� *DisplayControlNum+# [  $ 0��.+�0   *WDG.Wallet.Win.Views.Controls.UserPassWord  -     Name$	 pwd��$	��340��$��34��$
��Left=�$��Center=�   IsShowPassword �� *	IsChecked+	 �� ElementName$	 ck_isShowPwd��  �� 
   Password
 ��  �� Path$ PassWordData.PassWord� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� ValidationRules :   4WDG.Wallet.Win.ValidationRules.NotNullValidationRule  1   +WDG.Wallet.Win.ValidationRules.PasswordRule  ��   	tb_newPwd#���  $��2��$��Center=�$
��Left=�$��40 10 20 10q�� $��2��$��1��$��20 10q�.+�  -   $
 pwd1��$	��340��$��34��$
��Left=�$��Center=� �� *	IsChecked+$	 ck_isShowPwd��  �� 
 �� $ PassWordData.NewPassWord1�$ TwoWay=�$ PropertyChanged=�     �� $��3��  tb_confirmPwd#���  $��Center=�$
��Left=�$��40 10 20 10q�� $��3��$��1��$��20 10q�.+�  -   $
 pwd2��$	��340��$��34��$
��Left=�$��Center=� �� *	IsChecked+$	 ck_isShowPwd��  �� 
 �� $ PassWordData.NewPassWord2�$ TwoWay=�$ PropertyChanged=�     �� -    ��Name$ ck_isShowPwd��$��4��$��1��$��Center=�$��10 0 0 0q� $  page_pwdSetting_checkbox_viewPwd#���  $��34��$	��340��$��20 10q�$
��Left=��� $��5��$��2��$��Center=� �� Orientation$ 
Horizontal=�.+��� $��0q�$��80��
�������$��32��$��10q� 
 btn_OK#���  ���#44CC4Ey��� *DisplayControlNum+# [  $ 1������ *	OkCommand+��`�  	 _validationConverter `� 	Converter# [	 1��� $		 pwd���� $
	 pwd1���� $
	 pwd2��{�`�  
 _boolsToBoolMultiConverter# [
 1��� $
	 pwd1��$ (Validation.Errors)� ' #_listCountMoreThanToEnableConverter# [ $ 1���� $
	 pwd1��$ Password�  _isNullConverter# [ $ reverse���� $
	 pwd2��$ (Validation.Errors)�# [ $ 1���� $
	 pwd2��$ Password�# [ $ reverse���� $��0q�$��80��
�������$��32��$��10q�#���  ���#44CC4Ey��� *DisplayControlNum+# [  $ 0������ *	OkCommand+��`� # [	 1��� $		 pwd���� $
	 pwd1���� $
	 pwd2��{�`� # [
 1��� $		 pwd��$ (Validation.Errors)�# [ $ 1���� $		 pwd��$ Password�# [ $ reverse���� $
	 pwd1��$ (Validation.Errors)�# [ $ 1���� $
	 pwd1��$ Password�# [ $ reverse���� $
	 pwd2��$ (Validation.Errors)�# [ $ 1���� $
	 pwd2��$ Password�# [ $ reverse���� $��Center=�$��Center=�$��80�����#8C8C8C$��32��$��10q�$��1q����#D4D4D4  
btn_Cancel#���  
����������� *CancelCommand+
   M S B A M L   `   `   `  ���� I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WDG.Wallet.Win.ViewModelsWDG.Wallet.Win.ViewModels  M+clr-namespace:WDG.Wallet.Win.CustomControlsWDG.Wallet.Win.CustomControls  #    WDG.Wallet.Win.Views.SendPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6vm'clr-namespace:WDG.Wallet.Win.ViewModels     Fcustomcontrols+clr-namespace:WDG.Wallet.Win.CustomControls       N� Title$  SendPage�� � DataContext -   'WDG.Wallet.Win.ViewModels.SendViewModel  .��� 
������� � RowDefinitions �� $��* ��� $
��Auto ��� $
��Auto �.+��� ��. $��Hidden=�$
��Auto=�.���� -    ��Name$ items��   SendItemInfoDataTemplate#��[  ���� *	SendItems+�� $��1��$��30q�$��50�����#F0FAFF���#E2F2FA$��1q�._���  �� Orientation$ 
Horizontal=�.+���   Title_Tradefee#���  $��Center=�$��20 0 0 0q��� $��Center=�$��20 0 0 0q����� *Fee+ �� StringFormat$ {0:N8} WDG/kb����   	Btn_Chose#���  $��Center=�$��50 0 0 0q�$��80��$��28�����#3B8EFF
����������� *ChoseFeeCommand+��   Btn_SendSetting#���  /   )WDG.Wallet.Win.ViewModels.StaticViewModel   GlobalViewModel# Z $��Center=�$��50 0 0 0q�$��80��$
��Auto��$��28�����#3B8EFF
�������y��� *IsProfessional+ ! _booleanToVisibilityConverter �� 	Converter# [ ���� *SendSettingCommand+�� $��2��$	��5 0q�._��  � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $ 
Horizontal=�$
��30 5q�.+�.   (WDG.Wallet.Win.CustomControls.PathButton    path_btn_send	   PathData#	 [ $��6q�$	��100��$
��Auto��  btn_normal_ContentColor
   DefaultFillBrush#
 [   btn_mouseOver_ContentColor   MouseOverBrush# [   btn_enable_ContentColor   IsPressedBrush# [    IsEnabledBrush# [  	 Btn_Send#��� 	 $��0 0 10 0q�$��34�����#3B8EFF
����������� *SendCommand+����  �� ElementName$ items���� ���#D8D8D8$��1q� 
 ClearAll#��� 
 $��0 0 10 0q�$��34��$	��100��$
��Auto�����#F9F9F9���#8A8A8A���� *ClearCommand+��   Btn_AddPayee#���  ���#D8D8D8$��1q�$	��100��$
��Auto��$��0 0 10 0q�$��34�����#F9F9F9���#8A8A8A���� *
AddCommand+{��� *	SendItems+  _listCountToEnableConverter# [  �� ConverterParameter$ 20���� $��1 ��$ 
Horizontal=�$��Right=�$��0 0 30 0q�.+���   Title_Over1#���  $��Center=��� $��Center=����� *	OverMoney+  _longToDoubleConverter# [ $ {0} WDG��
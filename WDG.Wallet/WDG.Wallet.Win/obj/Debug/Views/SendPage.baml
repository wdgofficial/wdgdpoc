   M S B A M L   `   `   `  ����I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WDG.Wallet.Win.ViewModelsWDG.Wallet.Win.ViewModels  5      M+clr-namespace:WDG.Wallet.Win.CustomControlsWDG.Wallet.Win.CustomControls  5      #    WDG.Wallet.Win.Views.SendPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6vm'clr-namespace:WDG.Wallet.Win.ViewModels     5      Fcustomcontrols+clr-namespace:WDG.Wallet.Win.CustomControls     5        N� Title$  SendPage��5
       � DataContext 5      -   'WDG.Wallet.Win.ViewModels.SendViewModel  5   
   .��5      � 
������� � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� ��. $��Hidden=�6/   $
��Auto=�6V   .��5      �� -   5         ��Name$ items��5         SendItemInfoDataTemplate#��[  6L   ��6*   �� *	SendItems+6L   5      �� 5   
   $��1��6   $��30q�6   $��50��6*   ���#F0FAFF���#E2F2FA$��1q�6a   ._�5      ��  �� Orientation$ 
Horizontal=�6   .+�5      ��   Title_Tradefee#���  6   $��Center=�6D   $��20 0 0 0q�6_   �� 5      $��Center=�6N   $��20 0 0 0q�6i   ��6   �� *Fee+ �� StringFormat$ {0:N8} WDG/kb��6i   �� 5        	Btn_Chose#���  6   $��Center=�6?   $��50 0 0 0q�5      $��80��5      $��28��6$   ���#3B8EFF
���������5   +   �� *ChoseFeeCommand+5   E   �� 5'        Btn_SendSetting#���  6   /   )WDG.Wallet.Win.ViewModels.StaticViewModel   GlobalViewModel# Z 5(      $��Center=�5)      $��50 0 0 0q�5+      $��80��5,      $
��Auto��6'   $��28��64   ���#3B8EFF
�������y�5*      �� *IsProfessional+ ! _booleanToVisibilityConverter �� 	Converter# [ ��5+   +   �� *SendSettingCommand+5,   U   5-      5/      �� 51   
   $��2��6   $	��5 0q�6   ._�52      �  � ColumnDefinitions 53      �� 54      $��* �6'   �� 55      $
��Auto �6'   .+�57      �� $ 
Horizontal=�6   $
��30 5q�66   .+�58      .   (WDG.Wallet.Win.CustomControls.PathButton    path_btn_send	   PathData#	 [ 59      $��6q�5:      $	��100��6%   $
��Auto��64     btn_normal_ContentColor
   DefaultFillBrush#
 [ 5;        btn_mouseOver_ContentColor   MouseOverBrush# [ 5<        btn_enable_ContentColor   IsPressedBrush# [ 5=         IsEnabledBrush# [ 5>       	 Btn_Send#��� 	 5?   9   $��0 0 10 0q�5@   H   $��34��6Z   ���#3B8EFF
���������5?      �� *SendCommand+��5@      ��  �� ElementName$ items��6{   �� 5A      ���#D8D8D8$��1q�5B   3    
 ClearAll#��� 
 5C      $��0 0 10 0q�6B   $��34��6T   $	��100��6`   $
��Auto��6o   ���#F9F9F9���#8A8A8A��5A      �� *ClearCommand+5D   2   �� 5E        Btn_AddPayee#���  6<   ���#D8D8D8$��1q�5F   3   $	��100��6G   $
��Auto��6V   $��0 0 10 0q�5H      $��34��6/   ���#F9F9F9���#8A8A8A��5E      �� *
AddCommand+{�5G      �� *	SendItems+  _listCountToEnableConverter# [  �� ConverterParameter$ 20��5H   P   5I      �� 5J      $��1 ��6   $ 
Horizontal=�6.   $��Right=�6G   $��0 0 30 0q�6c   .+�5K      ��   Title_Over1#���  6    $��Center=�6E   �� 5L      $��Center=�6�   ��6    �� *	OverMoney+  _longToDoubleConverter# [ $ {0} WDG��6�   5M      5N      5O      5P      5Q      
   M S B A M L   `   `   `  ���� I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WDG.Wallet.Win.ViewModelsWDG.Wallet.Win.ViewModels  =#clr-namespace:WDG.Wallet.Win.ModelsWDG.Wallet.Win.Models  M+clr-namespace:WDG.Wallet.Win.CustomControlsWDG.Wallet.Win.CustomControls  O,clr-namespace:WDG.Wallet.Win.ValidationRulesWDG.Wallet.Win.ValidationRules  &     WDG.Wallet.Win.Views.ReceivePage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    >d2http://schemas.microsoft.com/expression/blend/2008    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6vm'clr-namespace:WDG.Wallet.Win.ViewModels     6models#clr-namespace:WDG.Wallet.Win.Models     Fcustomcontrols+clr-namespace:WDG.Wallet.Win.CustomControls     >rules,clr-namespace:WDG.Wallet.Win.ValidationRules       N� Title$  ReceivePage�� � DataContext 0   *WDG.Wallet.Win.ViewModels.ReceiveViewModel  .��� ���#F4F6F9.+�� $��20q� � RowDefinitions �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto � � ColumnDefinitions �� $
��Auto �$��70���� �� $
��Auto �.+��� $��1��$��0��   	Title_Tag#���   $
��Left=�$��5 50 0 5q�$��Center=��� $��2��$��0��  Mark#���  $
��Left=�$��5 2 5 2q�$��Center=��� $��3��$��0�� 
 Amount#���  $
��Left=�$��5 2 5 2q�$��Center=���  �� 	MaxLength$ 30��  WarkTextBoxStyle#��[   tooltip_Label
 � Tag# �  $��1��$��0 50 0 5q�$��34��$��1��$��Center=����� *ReceiveData.Tag+�� $	 200��#��[   tooltip_Mark# �  $��2��$��0 5 0 5q�$��34��$��1��$��Center=����� *ReceiveData.Comment+�� $��3��$��1�� �� Orientation$ 
Horizontal=�.+��� -    ��Name$ 	tb_amount��$	��0 5q�$��34��$	��240��$
��Left=�$��Center=�  AmountBoxStyle#��[ .������  �� Path$ ReceiveData.Amount_Str�	 �� Mode$	 TwoWay=�
 �� UpdateSourceTrigger$
 PropertyChanged=� �� FallbackValue$ 0�� �� ValidationRules 4   .WDG.Wallet.Win.ValidationRules.DoubleRangeRule  -   'WDG.Wallet.Win.ValidationRules.RuleBase   ErrorKey$ Error_Amount��
   Min$ 0��.   (WDG.Wallet.Win.ValidationRules.RegexRule     Pattern$# ([0-9]{1,10}(\.[0-9]{0,8})?)?��   ErrorMsg$ Error_Amount��.���� $	��WDG��$��Center=����#333333$��10 0 0 0q��� $ 
Horizontal=�$��4��$��1��$��0 15 0 0q�.+��� $	��100��$
��Auto��$��34��$
��10 0q�  Btn_RequestPay#���  ���� *RequestPayCommand+��`�   _validationConverter `� 	Converter# [ .1���  �� ElementName$ 	tb_amount���� $	��100��,   &WDG.Wallet.Win.Models.ReceiveClearType
   All#��Z $��34��$��15 0 0 0q� 	 ClearAll#��� 	 $	��Top=����� *ClearCommand+.   (WDG.Wallet.Win.CustomControls.PathButton  $��2��$��1��$��	11 50 0 5q�$��Center=�$��30��$��30�� 
 
path_Close   PathData# [
 $��3q�
   Tag#��Z ���� *ClearCommand+  $��2��$��2��$��11 5 0 5q�$��Center=�$��30��$��30��# [
 $��3q�   Comment#��Z ���� *ClearCommand+
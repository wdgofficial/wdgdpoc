   M S B A M L   `   `   `  ���� I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:WDG.Wallet.Win.Views.ShellPagesWDG.Wallet.Win.Views.ShellPages  [2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages$WDG.Wallet.Win.ViewModels.ShellPages  E'clr-namespace:WDG.Wallet.Win.ViewModelsWDG.Wallet.Win.ViewModels  E'clr-namespace:WDG.Wallet.Win.ConvertersWDG.Wallet.Win.Converters  M+clr-namespace:WDG.Wallet.Win.CustomControlsWDG.Wallet.Win.CustomControls  E AWDG.Models, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:WDG.Models;assembly=WDG.Models
WDG.Models 5    /WDG.Wallet.Win.Views.ShellPages.SendSettingPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ?local-clr-namespace:WDG.Wallet.Win.Views.ShellPages     Avm2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages     7vms'clr-namespace:WDG.Wallet.Win.ViewModels     >
converters'clr-namespace:WDG.Wallet.Win.Converters     @controls+clr-namespace:WDG.Wallet.Win.CustomControls     ?models,clr-namespace:WDG.Models;assembly=WDG.Models     $	��680��$	��400��  N� Title$  SendSettingPage�� � DataContext ?   9WDG.Wallet.Win.ViewModels.ShellPages.SendSettingViewModel  .��� 
������� � ColumnDefinitions �� $
��Auto ���  � RowDefinitions �� $��40 ��� $��60 ��� $��40 ��� $
��Auto �$��40���� $��40 ��� $��40 ��� $
��Auto �.+�2   ,WDG.Wallet.Win.Views.ShellPages.TitleControl     Btn_SendSetting   Title# �   $��2���� $��2��  setting_locktime#���  $��Center=�$��40 0 0 0q�(  "System.Windows.Controls.DatePicker  $��2��$��1��$��30��/   )WDG.Wallet.Win.ViewModels.StaticViewModel   GlobalViewModel# Z  � Language$ en-US�$
��Left=�$��Center=�$��Center=�$��	10 0 20 0q�  DefaultDatePicker#��[    SelectedDate �� *DataContext.Setting.LockTime+ �� RelativeSource �� 	 �� AncestorType5   /WDG.Wallet.Win.Views.ShellPages.SendSettingPage"	  y��� *	IsEnglish+ ! _booleanToVisibilityConverter
 �� 	Converter#
 [   $��2��$��1��$��30��# Z $
��Left=�$��Center=�$��Center=�$��	10 0 20 0q�#��[  �� *DataContext.Setting.LockTime+ �� "	  y��� *	IsEnglish+#
 [  �� ConverterParameter$ reverse���� $��3��  setting_chooesUTXO#���  $	��Top=�$��	40 12 0 0q��� $��1��$��3��.+�.   (WDG.Wallet.Win.CustomControls.PathButton  $��25��$��25��$
��Left=�$��10 7 0 0q�
������    PathData� �� ʚ; ʚ;�        � �0#�3;� Y�"�3;��N"���:`��!�ϒ:�����Dw1��n��1 >���0 >�@��/� >�@�� >�`G� �� s���Vh�`���h:
��O� (
` ��B
 �� ��	�@�} ��ffffff@@ W�ffffff@@�� �ffffff@@ � �j������#@��q������#@Q ~*4������#@S�07������#@�ӧ9 ��ӧ9�� Ӡӧ9@Ë`9 #���%8@�	���8���	��8@�
���7�j
��DB%���Ӏ�7%�`�/% J`�/%�'�`�/%���8�`�/% X�9���$@�x:�
$���:�@S�#�j�: z#�3; �0#�3;� �0#�3;���� *ChooseUtxoCommand+�� ���� *Setting.UTXO+���� .R�$�  $� Orientation$ Vertical=�����  �� DataType  WDG.Models.PageUnspent"  .R���  �� Orientation$ 
Horizontal=�.+��� $��Center=�$��10 0 0 0q����� 0   *WDG.Wallet.Win.Converters.StaticConverters   ModelToStringConverter#
 Z 5	   /WDG.Wallet.Win.Converters.StaticConverterParams 	  ModelType_PageUnspent# Z     path_main_noNet# [    
ImageWidth$ 20Q�   ImageHeight$ 20Q�$��30��$��30��$��5q� 
 Delete � ToolTip# �  ���� *DataContext.RemoveUtxoCommand+ �� $	 local:SendSettingPageL����� �� $��4��  setting_chooesChooseAddress#���  $��Center=�$��40 0 0 0q��� $��4��$��1��$ 
Horizontal=�$��Center=�.+��� $��Center=�$��10 0 0 0q����� *Setting.ChangeAddress+#
 Z  	  ModelType_AccountInfo# Z   $��25��$��25��
������ � �� ʚ; ʚ;�        � �0#�3;� Y�"�3;��N"���:`��!�ϒ:�����Dw1��n��1 >���0 >�@��/� >�@�� >�`G� �� s���Vh�`���h:
��O� (
` ��B
 �� ��	�@�} ��ffffff@@ W�ffffff@@�� �ffffff@@ � �j������#@��q������#@Q ~*4������#@S�07������#@�ӧ9 ��ӧ9�� Ӡӧ9@Ë`9 #���%8@�	���8���	��8@�
���7�j
��DB%���Ӏ�7%�`�/% J`�/%�'�`�/%���8�`�/% X�9���$@�x:�
$���:�@S�#�j�: z#�3; �0#�3;� �0#�3;���� *ChooseAddrCommand+�� $��5��  setting_isenable#���  $��Center=�$��40 0 0 0q��� $��5��$��1��$
��Auto��$��10 0 0 0q�$��Center=� d� 	IsChecked �� *Setting.IsEnable+�� $��6��$��2��$��0 0 0 0q�$��Center=�$ 
Horizontal=�$��Center=�.+��� $��3��$��2��$��0q�$��80��
�������$��32��$��10q� 
	 btn_OK#��� 	 ���#44CC4E���� *	OkCommand+�� $��3��$��2��$��Center=�$��Center=�$��80�����#8C8C8C$��32��$��10q�$��1q����#D4D4D4 
 
btn_Cancel#��� 
 
����������� *CancelCommand+
   M S B A M L   `   `   `  ����I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:WDG.Wallet.Win.Views.ShellPagesWDG.Wallet.Win.Views.ShellPages  5      [2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages$WDG.Wallet.Win.ViewModels.ShellPages  5      E'clr-namespace:WDG.Wallet.Win.ViewModelsWDG.Wallet.Win.ViewModels  5      E'clr-namespace:WDG.Wallet.Win.ConvertersWDG.Wallet.Win.Converters  5	      M+clr-namespace:WDG.Wallet.Win.CustomControlsWDG.Wallet.Win.CustomControls  5
      E AWDG.Models, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:WDG.Models;assembly=WDG.Models
WDG.Models 5      5    /WDG.Wallet.Win.Views.ShellPages.SendSettingPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:WDG.Wallet.Win.Views.ShellPages     5      Avm2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages     5      7vms'clr-namespace:WDG.Wallet.Win.ViewModels     5      >
converters'clr-namespace:WDG.Wallet.Win.Converters     5	      @controls+clr-namespace:WDG.Wallet.Win.CustomControls     5
      ?models,clr-namespace:WDG.Models;assembly=WDG.Models     5      $	��680��5      $	��400��6     N� Title$  SendSettingPage��5       � DataContext 5      ?   9WDG.Wallet.Win.ViewModels.ShellPages.SendSettingViewModel  5   
   .��5      � 
������� � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5       � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��60 �6   �� 5      $��40 �6   �� 5      $
��Auto �6   $��40��6*   �� 5      $��40 �6   �� 5      $��40 �6   �� 5      $
��Auto �6   .+�5!   
   2   ,WDG.Wallet.Win.Views.ShellPages.TitleControl     Btn_SendSetting   Title# �   6   $��2��6G   �� 5#   
   $��2��6     setting_locktime#���  6!   $��Center=�6K   $��40 0 0 0q�6f   (  "System.Windows.Controls.DatePicker  5$   
   $��2��6   $��1��6"   $��30��62   /   )WDG.Wallet.Win.ViewModels.StaticViewModel   GlobalViewModel# Z 5&       � Language$ en-US�5(      $
��Left=�5)      $��Center=�60   $��Center=�6K   $��	10 0 20 0q�5*        DefaultDatePicker#��[ 6(      SelectedDate 5%      �� *DataContext.Setting.LockTime+ �� RelativeSource �� 	 �� AncestorType5   /WDG.Wallet.Win.Views.ShellPages.SendSettingPage"	  y�5'      �� *	IsEnglish+ ! _booleanToVisibilityConverter
 �� 	Converter#
 [ 5*   (     5,   
   $��2��6   $��1��6"   $��30��62   # Z 5.      $
��Left=�50      $��Center=�60   $��Center=�6K   $��	10 0 20 0q�51      #��[ 6(    5-      �� *DataContext.Setting.LockTime+ �� "	  y�5/      �� *	IsEnglish+#
 [  �� ConverterParameter$ reverse��51   (   �� 52   
   $��3��6     setting_chooesUTXO#���  6!   $	��Top=�6M   $��	40 12 0 0q�6e   �� 53   
   $��1��6   $��3��6%   .+�54      .   (WDG.Wallet.Win.CustomControls.PathButton  $��25��55      $��25��6   $
��Left=�56      $��10 7 0 0q�6,   
������    PathData� �� ʚ; ʚ;�        � �0#�3;� Y�"�3;��N"���:`��!�ϒ:�����Dw1��n��1 >���0 >�@��/� >�@�� >�`G� �� s���Vh�`���h:
��O� (
` ��B
 �� ��	�@�} ��ffffff@@ W�ffffff@@�� �ffffff@@ � �j������#@��q������#@Q ~*4������#@S�07������#@�ӧ9 ��ӧ9�� Ӡӧ9@Ë`9 #���%8@�	���8���	��8@�
���7�j
��DB%���Ӏ�7%�`�/% J`�/%�'�`�/%���8�`�/% X�9���$@�x:�
$���:�@S�#�j�: z#�3; �0#�3;� �0#�3;��54   "   �� *ChooseUtxoCommand+58      �� 59      ��6   �� *Setting.UTXO+��5:      �� 5;      .R�5<      $�  $� Orientation$ Vertical=�61   5=      ��5?      �� 5@       �� DataType  WDG.Models.PageUnspent"  6#   .R�5A      ��  �� Orientation$ 
Horizontal=�6%   .+�5B      �� $��Center=�5C      $��10 0 0 0q�67   ��5B   (   �� 0   *WDG.Wallet.Win.Converters.StaticConverters   ModelToStringConverter#
 Z 5	   /WDG.Wallet.Win.Converters.StaticConverterParams 	  ModelType_PageUnspent# Z 5C   7     5D        path_main_noNet# [ 62      
ImageWidth$ 20Q�5E   2      ImageHeight$ 20Q�6B   $��30��5F   2   $��30��6=   $��5q�6I    
 Delete � ToolTip# �  6U   ��5G   2   �� *DataContext.RemoveUtxoCommand+ �� $	 local:SendSettingPageL���5H   2   �� 5I      5J      5L      5M      �� 5O   
   $��4��6     setting_chooesChooseAddress#���  6!   $��Center=�6V   $��40 0 0 0q�6q   �� 5Q   
   $��4��6   $��1��6"   $ 
Horizontal=�62   $��Center=�6K   .+�5R      �� $��Center=�5S      $��10 0 0 0q�63   ��5R      �� *Setting.ChangeAddress+#
 Z  	  ModelType_AccountInfo# Z 5S   3     5T      $��25��5U      $��25��6   
������ � �� ʚ; ʚ;�        � �0#�3;� Y�"�3;��N"���:`��!�ϒ:�����Dw1��n��1 >���0 >�@��/� >�@�� >�`G� �� s���Vh�`���h:
��O� (
` ��B
 �� ��	�@�} ��ffffff@@ W�ffffff@@�� �ffffff@@ � �j������#@��q������#@Q ~*4������#@S�07������#@�ӧ9 ��ӧ9�� Ӡӧ9@Ë`9 #���%8@�	���8���	��8@�
���7�j
��DB%���Ӏ�7%�`�/% J`�/%�'�`�/%���8�`�/% X�9���$@�x:�
$���:�@S�#�j�: z#�3; �0#�3;� �0#�3;��5T   #   �� *ChooseAddrCommand+5W      5X      �� 5Y   
   $��5��6     setting_isenable#���  6!   $��Center=�6K   $��40 0 0 0q�6f   �� 5Z   
   $��5��6   $��1��6    $
��Auto��60   $��10 0 0 0q�6e   $��Center=�6w    d� 	IsChecked 6>   �� *Setting.IsEnable+6w   �� 5]   
   $��6��6   $��2��6"   $��0 0 0 0q�66   $��Center=�6G   $ 
Horizontal=�6b   $��Center=�6{   .+�5^      �� $��3��6   $��2��6"   $��0q�67   $��80��5_      
�������$��32��6:   $��10q�6G    
	 btn_OK#��� 	 5`      ���#44CC4E��6Q   �� *	OkCommand+�� 5b      $��3��6   $��2��6"   $��Center=�66   $��Center=�6Q   $��80��5c      ���#8C8C8C$��32��6<   $��10q�6H   $��1q�6T   ���#D4D4D4 
 
btn_Cancel#��� 
 5d      
���������6S   �� *CancelCommand+5e      5f      5g      
   M S B A M L   `   `   `  ����I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:WDG.Wallet.Win.Views.ShellPagesWDG.Wallet.Win.Views.ShellPages  5      [2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages$WDG.Wallet.Win.ViewModels.ShellPages  5      O,clr-namespace:WDG.Wallet.Win.ValidationRulesWDG.Wallet.Win.ValidationRules  5      6    0WDG.Wallet.Win.Views.ShellPages.CreatePayUrlPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:WDG.Wallet.Win.Views.ShellPages     5      Avm2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages     5      >rules,clr-namespace:WDG.Wallet.Win.ValidationRules     5      $	��220��5      $	��680��6     N� Title$  CreatePayUrlPage��5       � DataContext 5      @   :WDG.Wallet.Win.ViewModels.ShellPages.CreatePayUrlViewModel  5   
   .��5      � 
������� � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5       � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      .+�5   
   2   ,WDG.Wallet.Win.Views.ShellPages.TitleControl  $��2��6t      Title 6   �� *UrlInfo.Mode+    _urlInfoModeToTitleConverter �� 	Converter# [  6t   �� 5   
   $��1��6     Tag#���  6!   $��Center=�6>   $��	20 20 0 5q�6Y   �� 5   
   $��1��6    �� 	MaxLength$ 30��6   $��1��6K     WarkTextBoxStyle#��[ 6[   
 � Tag# �  6�   $��Center=�6�   $��32��6�   $��Center=�6�   $��
10 20 50 5q�6�   ��6.   �� *UrlInfo.Tag+6�   �� 5!   
   $��2��6     Address#���  6!   $��Center=�6B   $��20 5 0 5q�6]   �� 5"   
   -   5         ��Name$ 
addressBox��5"      $��2��6&   $��1��5#        AddressWartMark# �  6"   #��[ 6J   $��Center=�6t   $��32��6�   $��Center=�6�   $��	10 5 50 5q�6�   {�5"   3   �� *IsEditAddress+.��5#   �   ��5$      �� 5%      	 �� Path$	 UrlInfo.Address�6   
 �� Mode$
 TwoWay=�61    �� UpdateSourceTrigger$ PropertyChanged=�6?    �� ValidationRules 5&      .   (WDG.Wallet.Win.ValidationRules.RegexRule  5'         ErrorMsg$ Error_Address��6*      Pattern$" (wdgmm|wdgtt)[0-9a-zA-Z]{33}��6C   5)      .��5*      5+      �� 5-   
   $��4��6   $��2��6"    �� Orientation$ 
Horizontal=�66   $��Center=�6O   $��Bottom=�6l   $��0 0 0 30q�6�   .+�5.      ��  
 btn_OK#���  6   $��32��6V   $��80��6b   $��0 0 5 0q�6m   ��68   �� *	OkCommand+��5/      `� 50        _validationConverter `� 	Converter# [ 6$   .1�51      ��  �� ElementName$ 
addressBox��6"   52      54      �� 55        
btn_Cancel#���  6   
����������#8C8C8C$��1q�6�   ���#D4D4D4$��32��6�   $��80��6�   $��5 0 0 0q�6�   ��6<   �� *CancelCommand+6�   56      58      59      
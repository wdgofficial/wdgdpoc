   M S B A M L   `   `   `  ����I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null[2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages$WDG.Wallet.Win.ViewModels.ShellPages  5      Q-clr-namespace:WDG.Wallet.Win.Views.ShellPagesWDG.Wallet.Win.Views.ShellPages  5      0    *WDG.Wallet.Win.Views.ShellPages.OptionPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      >d2http://schemas.microsoft.com/expression/blend/2008    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Avm2clr-namespace:WDG.Wallet.Win.ViewModels.ShellPages     5      ?local-clr-namespace:WDG.Wallet.Win.Views.ShellPages     5      $	��290��5	      $	��550��6     � DataContext  5
      :   4WDG.Wallet.Win.ViewModels.ShellPages.OptionViewModel  5   
   .��5      � 
������� � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��* �6   .+�5   
   2   ,WDG.Wallet.Win.Views.ShellPages.TitleControl     page_option   Title# �   6   �� 5   
   $��1��6    �� CornerRadius$ 0 0 1 1��6   ._�5      �  � ColumnDefinitions 5      �� 5      $
��Auto �6'   �� 5       5      �� 5      $��50 �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   .+�5"      ��   page_Option_cb_choseModel#���  6   $��1��6O   $��Center=�6\   $��Right=�6w   $��40 10 20 10q�6�   �� 5$       ) %page_languageSetting_cb_choseLanguage#���  6   $��2��6[   $��Center=�6h   $��Right=�6�   $��40 10 20 10q�6�   �� 5'      $��1��6   $��1��6(   $��Center=�6e   $��10 0 0 0q�5(      $��34��5)      $	��340��6'   $��20 10q�63   $
��Left=�6B    �� SelectedIndex 5'   8   �� *ModelSelectedIndex+.@�5*      ��   page_Option_cbi_default#���  6#   �� 5+        page_Option_cbi_profession#���  6#   5,      �� 5.      $��2��6   $��1��6(   $��Center=�6`   $��10 0 0 0q�5/      $��34��50      $	��340��6'   $��20 10q�63   $
��Left=�6B    5.   8   �� *SelectedIndex+.@�51      �� .��6#   简体中文6)   �� 52      .��6#   	English6,   53      �� 56      $��3��6   $��2��6*   $��Center=�6>    �� Orientation$ 
Horizontal=�6[   $��0 10 0 0q�6t   .+�57      �� $��0q�6   $��80��58      
�������$��32��6:   $��10q�6G    
 btn_OK#���  59      ���#44CC4E��6Q   �� *	OkCommand+�� 5;      $��Center=�6   $��Center=�68   $��1q�6U   ���#D4D4D4$��80��5<      ���#8C8C8C$��32��6<   $��10q�6H     
btn_Cancel#���  5=      
���������6S   �� *CancelCommand+5>      5?      5@      5A      5B      
   M S B A M L   `   `   `  ����I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null/clr-namespace:WDG.Wallet.WinWDG.Wallet.Win  5      E'clr-namespace:WDG.Wallet.Win.ViewModelsWDG.Wallet.Win.ViewModels  5      E'clr-namespace:WDG.Wallet.Win.ConvertersWDG.Wallet.Win.Converters  5      a5clr-namespace:WDG.Wallet.Win.CustomControls.Waittings'WDG.Wallet.Win.CustomControls.Waittings  5	          WDG.Wallet.Win.InitPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .localclr-namespace:WDG.Wallet.Win     5      6vm'clr-namespace:WDG.Wallet.Win.ViewModels     5      >
converters'clr-namespace:WDG.Wallet.Win.Converters     5      Jwaitctls5clr-namespace:WDG.Wallet.Win.CustomControls.Waittings     5	      c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5        N� Title$  InitPage��5       � DataContext 5      -   'WDG.Wallet.Win.ViewModels.InitViewModel  5   
   .  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ Loaded��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      6  0System.Windows.Interactivity.InvokeCommandAction     Command 6$   �� *!DataContext.OnPageLoadedCommand+ �� RelativeSource ��  �� AncestorType$ local:InitPageL�5       � 	Resources 5         	fontColor	 �� Color  	backColor
 �� 
StartPoint �� EndPoint � Color � Offset��%�   &	        &	      �� 5   
   $	 White��6,   �� 5   
   $	
 0,0&�60   $	 1,0&�6A   .��5      � � 5      $ #100A19��6   $	 0.0Q�6/   � 5      $ #401514��6   $	 1.0Q�6/   5      5      5!      .��5"      � #��[ 6    � RowDefinitions 5#   
   �� 5$      �� 5%      .+�5'   
   �� $
��Fill=�6   $��2��6   $��images/back_init1.png��60   �� 5(   
   $	��Top=�6    �� Orientation$ 
Horizontal=�6-   $��
40 40 0 0 q�6F   .+�5)      �� $��50��6   $��50��6   $��images/logo1.png��6+   �� 5*      $��Winner Dynasty Group��6   
�������$��15 0 0 0q�6G   $��Center=�6Y   $��Center=�6t   $��26(�6�   $��PingFangSC-Regular)�6�   � 5+      $��Center=�6   $y�	Collapsed=�65   .+�5,      ��   	Version_T#���  6   ���#498BF4$��Center=�6T   $��16(�6q   $��PingFangSC-Regular)�6   �� 5-      $��0.1.0��6   ���#498BF4$��Center=�6>   $��16(�6[   $��PingFangSC-Regular)�6i   5.      5/      � 51   
   $��2��6    � ColumnDefinitions 52      �� 53      �� 54      $	��600��6#    56      �� 57      �� 58      $
��Auto �6    5:      � 5<   
   $��2��6   $��50 0 0 0q�6    $��Center=�62   $��Italic$�6M   $��PingFangSC-Regular)�6l   $��30(�6�   $
��Bold"�6�   #��[  6�   .+�5=      ��   init_caption1#���  6   $��	50 10 0 0q�6?   �� 5>        init_caption2#���  6   $��	50 80 0 0q�6?   �� 5?      $ 
Horizontal=�6   $��
50 150 0 0q�62   .+�5@      D   >WDG.Wallet.Win.CustomControls.Waittings.CirclePointRingLoading  ���#2BD5E9$��18��6G   $��18��6R   $��Center=�6^   �� 5A      $��16(�6   $��Normal"�6*   $��Normal$�6>   #��[  6Q   $��	10 0 0 -4q�6y   $��Center=�5B      /   )WDG.Wallet.Win.ViewModels.StaticViewModel   GlobalViewModel# Z 5C      ��5D      �� *
InitStep+0   *WDG.Wallet.Win.Converters.StaticConverters    InitStepToStringConverter �� 	Converter# Z 5F      5G      �� 5I   
   ���#498BF4$��2��6F   $��Center=�5J      $��Center=�61   $y�	Collapsed=�6L   $��21(�5K      $��PingFangSC-Regular)�6"   ��5I      �� *Msg.Message+5K   "   �� 5M   
   $��Bottom=�6   $��Center=�6/   $��2��6L   $��0 0 0 22q�6Y   $��Normal$�5N      $��Normal"�6'   $��16(�6;   ���#CDBEFF$��V 1.0.2��5O   )   5P      5Q      
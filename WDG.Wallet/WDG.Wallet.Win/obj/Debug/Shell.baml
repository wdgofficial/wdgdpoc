   M S B A M L   `   `   `  ����I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:WDG.Wallet.Win.CustomControlsWDG.Wallet.Win.CustomControls  5      E'clr-namespace:WDG.Wallet.Win.ViewModelsWDG.Wallet.Win.ViewModels  5	      a5clr-namespace:WDG.Wallet.Win.CustomControls.Waittings'WDG.Wallet.Win.CustomControls.Waittings  5
          WDG.Wallet.Win.Shell   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      Q MGFramework.BlankWindow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'blankWindowhttp://GFramework.org  5      c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      :d2http://schemas.microsoft.com/expression/blend/2008  5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @controls+clr-namespace:WDG.Wallet.Win.CustomControls     5      6vm'clr-namespace:WDG.Wallet.Win.ViewModels     5	      K	customers5clr-namespace:WDG.Wallet.Win.CustomControls.Waittings     5
      
�������  � WindowStartupLocation$  CenterScreen=�5   >      
Main_Title � Title# �   5      $	��720��6?   $
��1000��6O   $	��720��6_   $	��900��6l    � DataContext 5      .   (WDG.Wallet.Win.ViewModels.ShellViewModel  5   
   .  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ Loaded��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      6  0System.Windows.Interactivity.InvokeCommandAction     CommandParameter$ OnLoaded��6F      Command 6$   �� *WindowCommand+6F   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      .+�5   
   � ���#F5F5F5$��30��6$   (  "GFramework.BlankWindow.BlankWindow	   	Draggable	 . .+�5      o� $��0��6   $��30��5      $
��Auto��6   $��15 0 0 0q�5      
������ $	��Top=�5!      $
��Left=�5"       
 n� UsesItemContainerTemplate
 . $��Center=�5$       & "_menuItemContainerTemplateSelector$ n� ItemContainerTemplateSelector# [ 5%      y�5      �� *	IsIniting+ ! _booleanToVisibilityConverter �� 	Converter# [  �� ConverterParameter$ Reverse����5&      �� *	MenuItems+5'      �� 5)      $��Center=�6   $��Center=�60   y�5*      �� *	IsIniting+# [ $ Reverse��._�5,      �� $��Testnet��6   $
��Auto��6,   $��Center=�69   $��Center=�6T   $��0q�5.      $
��Bold"�6$   y�5-      �� *	IsTestNet+# [ 5.   $   5/      �� 51      $��20 0 0 0q�6    �� Orientation$ 
Horizontal=�6+   $
��Left=�6D   y�6_   �� *	IsIniting+# [ .+�52      �� #���   6   $
��Auto��6A   $��Center=�53      �� 54      $��	(Testnet)��6   $
��Auto��6.   $��Center=�56      $��5 0 0 0q�63   y�55      �� *	IsTestNet+# [ 56   3   57      �� 58      $ 
Horizontal=�6   $��Right=�62   .+�59      /   )WDG.Wallet.Win.CustomControls.ImageButton  $��30��6'   $��30��62      Source$# images/windows/window_min.png��6>   
������ $	��Min��6�   ��6~   �� *WindowCommand+6�     5:      $��30��6'   $��30��62   $# images/windows/window_max.png��6>   
������ $	��Max��6�   ��6~   �� *WindowCommand+6�     5;      $��30��6'   $��30��62   $% images/windows/window_close.png��6>   
������ $��Close��6�   ��6�   �� *WindowCommand+6�   5<      5=       � 5?   
   $��1��6   
�������  � NavigationUIVisibility$ Hidden=�6O   ��6/   �� *ContentView+6O    � 5A   
   $��2��6   $ Hidden=�5B      ��5A   !   �� *	PopupView+y�5C      �� *IsShowPopupView+# [ �� 5E   
   
������ 	 . y�5F      �� *IsShowPopupView+# [ 5G      �� 5I   
   
������ $��2��6*   y�5J      �� *IsShowMessage+# [ �� 5L   
   $��2��6    �� CornerRadius$ 4��6"   $	��240��5M      $	��120��6    ���#979797$
��Auto��6E   $
��Auto��6R   $��Center=�5N      $��Center=�6.   y�5O      �� *IsShowMessage+# [ ._�5P      �� $
��Wrap=�61   $��20q�6E   $��Center=�5Q      $��Center=�61   
���������5P      �� *	Message+5Q   N   5R      � 5V   
   
�������$��10��6!   $��5��63    J� Opacity$	 0.2Q�6G   /   )WDG.Wallet.Win.ViewModels.StaticViewModel   GlobalViewModel# Z 5W       K� ZIndex$ 2��6K   y�5X      �� *	IsLoading+# [ ?	   9WDG.Wallet.Win.CustomControls.Waittings.FiveColumnLoading	  5Z   
   # Z 6&   $��10��5[      $��5��6   $ 2��63   ���#3ca9fe$��Center=�5\   #   $��Center=�6>   y�5]      �� *	IsLoading+# [ 5^      5_      
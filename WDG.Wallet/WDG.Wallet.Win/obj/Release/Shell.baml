   M S B A M L   `   `   `  ���� I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:WDG.Wallet.Win.CustomControlsWDG.Wallet.Win.CustomControls  E'clr-namespace:WDG.Wallet.Win.ViewModelsWDG.Wallet.Win.ViewModels  a5clr-namespace:WDG.Wallet.Win.CustomControls.Waittings'WDG.Wallet.Win.CustomControls.Waittings      WDG.Wallet.Win.Shell   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    Q MGFramework.BlankWindow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'blankWindowhttp://GFramework.org  c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  :d2http://schemas.microsoft.com/expression/blend/2008  R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @controls+clr-namespace:WDG.Wallet.Win.CustomControls     6vm'clr-namespace:WDG.Wallet.Win.ViewModels     K	customers5clr-namespace:WDG.Wallet.Win.CustomControls.Waittings     
�������  � WindowStartupLocation$  CenterScreen=�   
Main_Title � Title# �   $	��720��$
��1000��$	��720��$	��900�� � DataContext .   (WDG.Wallet.Win.ViewModels.ShellViewModel  .  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ Loaded��.  (System.Windows.Interactivity.TriggerBase   Actions. 6  0System.Windows.Interactivity.InvokeCommandAction     CommandParameter$ OnLoaded��   Command �� *WindowCommand+.���  � RowDefinitions �� $
��Auto ��� .+�� ���#F5F5F5$��30��(  "GFramework.BlankWindow.BlankWindow	   	Draggable	 . .+�o� $��0��$��30��$
��Auto��$��15 0 0 0q�
������ $	��Top=�$
��Left=� 
 n� UsesItemContainerTemplate
 . $��Center=� & "_menuItemContainerTemplateSelector$ n� ItemContainerTemplateSelector# [ y��� *	IsIniting+ ! _booleanToVisibilityConverter �� 	Converter# [  �� ConverterParameter$ Reverse������ *	MenuItems+�� $��Center=�$��Center=�y��� *	IsIniting+# [ $ Reverse��._��� $��Testnet��$
��Auto��$��Center=�$��Center=�$��0q�$
��Bold"�y��� *	IsTestNet+# [ �� $��20 0 0 0q� �� Orientation$ 
Horizontal=�$
��Left=�y��� *	IsIniting+# [ .+��� #���   $
��Auto��$��Center=��� $��	(Testnet)��$
��Auto��$��Center=�$��5 0 0 0q�y��� *	IsTestNet+# [ �� $ 
Horizontal=�$��Right=�.+�/   )WDG.Wallet.Win.CustomControls.ImageButton  $��30��$��30��   Source$# images/windows/window_min.png��
������ $	��Min������ *WindowCommand+  $��30��$��30��$# images/windows/window_max.png��
������ $	��Max������ *WindowCommand+  $��30��$��30��$% images/windows/window_close.png��
������ $��Close������ *WindowCommand+ � $��1��
�������  � NavigationUIVisibility$ Hidden=����� *ContentView+ � $��2��$ Hidden=����� *	PopupView+y��� *IsShowPopupView+# [ �� 
������ 	 . y��� *IsShowPopupView+# [ �� 
������ $��2��y��� *IsShowMessage+# [ �� $��2�� �� CornerRadius$ 4��$	��240��$	��120�����#979797$
��Auto��$
��Auto��$��Center=�$��Center=�y��� *IsShowMessage+# [ ._��� $
��Wrap=�$��20q�$��Center=�$��Center=�
����������� *	Message+� 
�������$��10��$��5�� J� Opacity$	 0.2Q�/   )WDG.Wallet.Win.ViewModels.StaticViewModel   GlobalViewModel# Z  K� ZIndex$ 2��y��� *	IsLoading+# [ ?	   9WDG.Wallet.Win.CustomControls.Waittings.FiveColumnLoading	  # Z $��10��$��5��$ 2�����#3ca9fe$��Center=�$��Center=�y��� *	IsLoading+# [ 
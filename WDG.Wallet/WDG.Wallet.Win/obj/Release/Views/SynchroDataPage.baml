   M S B A M L   `   `   `  ���� I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WDG.Wallet.Win.ViewModelsWDG.Wallet.Win.ViewModels  Q-clr-namespace:WDG.Wallet.Win.Views.ShellPagesWDG.Wallet.Win.Views.ShellPages  *    $WDG.Wallet.Win.Views.SynchroDataPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6vm'clr-namespace:WDG.Wallet.Win.ViewModels     ?views-clr-namespace:WDG.Wallet.Win.Views.ShellPages     $	��280��$	��680��  N� Title$  SynchroDataPage�� � DataContext 4   .WDG.Wallet.Win.ViewModels.SynchroDataViewModel  .��� 
������� � RowDefinitions �� $��40 ��� $��* �.+�2   ,WDG.Wallet.Win.Views.ShellPages.TitleControl     synchronizing   Title# �      CloseIsWord .  
 shadow   	CloseWord# �  �� $��1�� �� CornerRadius$ 0 0 1 1��._��  � ColumnDefinitions �� $	��140 ���  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��20 10q�$��Center=�$
��Wrap=�  remainingblocksCount#���  �� $��1��$��20 10q�$��Center=�$
��Wrap=�  stratDataTime#���  �� $��2��$��20 10q�$��Center=�$
��Wrap=�  Rate#���  �� $��3��$��20 10q�$��Center=�$
��Wrap=�  STRS#���  �� $��1��$��0��$��20 10q�$��Center=�$
��Wrap=����� *BlockSyncInfo.BlockLeft+�� $��1��$��1��$��20 10q�$��Center=�$
��Wrap=����� *BlockSyncInfo.StartTimeOffset+ ! _timestampToDateTimeConverter �� 	Converter# [ 	 �� ConverterParameter$	 yyyy-MM-dd HH:mm:ss���� $��1��$��2��
 �� Orientation$
 
Horizontal=�$��0,3,0,33q�$��2��.+��� $��
20 10 0 10q�$��Center=�$
��Wrap=����� *BlockSyncInfo.Progress+ �� StringFormat$ {0:N2} %��� $��10 0 0 0q�$��20��$	��320�� � Value �� *BlockSyncInfo.Progress+�� $��1��$��3��$��20 10q�$��Center=�$
��Wrap=����� *BlockSyncInfo.TimeLeft+ " _timestampToTimeRangeConverter# [ 
   M S B A M L   `   `   `  ����I  EWDG.Wallet.Win, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:WDG.Wallet.Win.AttachesWDG.Wallet.Win.Attaches  5      U/clr-namespace:WDG.Wallet.Win.resources.Controls!WDG.Wallet.Win.resources.Controls  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source   DefaultDatePicker �� 
TargetType(   "System.Windows.Controls.DatePicker �� Property  TextForeground �� Value  TextBackground  ControlBorderBrush3   -WDG.Wallet.Win.Attaches.ControlAttachProperty  FocusBorderBrush  MouseOverBorderBrush  
FontFamily  FontSize  DefaultCalendar �� 
TargetType �Name ��Name J� SnapsToDevicePixels	 � ColumnDefinitions
 ��Name   LabelTemplate J� IsHitTestVisible   Label   AttachContent d�Name 	 FIconToggleButton   	FIconSize   FocusBackground d� 	IsChecked �� Mode �� RelativeSource ��Name �� Padding   	Watermark 
 WatermarkOpacity J� Opacity �� TextAlignment ��Name  EditableTextBoxStyle �� 
IsReadOnly �� Path  
DateFormat �� StringFormat �Name � AllowsTransparency  � 	StaysOpen! � PlacementTarget" �� ElementName# �� Triggers$ �� Value% �� Binding& �� 
TargetName' P� Property( P� Value  DisableOpacity  ClearButtonDatePicker) �� BasedOn/   )WDG.Wallet.Win.resources.Controls.FButton*   FIcon  FButton_Transparency+   FIconMargin',    IsClearTextButtonBehaviorEnabled-   	FIconSize. � Cursor/ �� AncestorType  LabelDatePicker  TextLabelBackground  LabelClearButtonDatePicker�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    0attach%clr-namespace:WDG.Wallet.Win.Attaches  5      <controls/clr-namespace:WDG.Wallet.Win.resources.Controls  5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5        5      �� 5   
   $Z Tpack://application:,,,/WDG.Wallet.Win;component/resources/DateTimeThemes/Colors.xaml?�6   �� 5   
   $\ Vpack://application:,,,/WDG.Wallet.Win;component/resources/DateTimeThemes/Calendar.xaml?�6   �� 5   
   $` Zpack://application:,,,/WDG.Wallet.Win;component/resources/DateTimeThemes/ToggleButton.xaml?�6   �� 5	   
   $Y Spack://application:,,,/WDG.Wallet.Win;component/resources/DateTimeThemes/Share.xaml?�6   %N  &	        7  7  7  7  7  7  7  7  7 	 7 
 7  7  7  &	 �    7   7  7  &	 1    7   7  &	 ^    7   7  7  7  �� 5      "   6&   .�5   
   ��  � ��8   6'   �� 5   
    � ��8  6'   �� 5   
    � ��$ 1q�6,   �� 5   
    � ��8  6(   �� 5   
    �@ FocusBorderBrush8  6J   �� 5   
    �@ MouseOverBorderBrush8  6N   �� 5   
    �@ 	FIconSize$ 18Q�6C   �� 5   
    � ��8  6'   �� 5   
    � ��8  6%   �� 5   
    � ��$ 26��6&   �� 5   
    � ��$ 30��6#   �� 5   
    � ��$	 200��6"   �� 5   
    � ��$ Stretch=�67   �� 5   
    �@  IsTodayHighlighted . 6/   �� 5   
    � ��$ 0q�6$   �� 5   
    � ��$ 0q�6#   �� 5   
    �@  SelectedDate 6)   Z� �� 5   
    �@J�SnapsToDevicePixels . 60   �� 5   
    �@  CalendarStyle8  6*   �� 5    
    � �� 5!      �� 5"      "   6"   .R�5#      � $ 	PART_Root��6   .+�5$      �� $ Bg��6!   # z 6-   #��z��5%   !   #��z��6M   #��z��5&   !   � 5'      $ PART_InnerGrid��6   $��0q�67   	 5(      �� 5)   "   $
��Auto �63   �� 5*   "   $��* �63   �� 5+   "   $
��Auto �63   �� 5,   "   $
��Auto �63   .+�5/      �� $
 Label��6-   #��z 6<   ��.   .  #��z 50   -   $��1,1,0,1q�6l   �� 52      $ PART_AttachContent��6%   $��2��6A   $��Center=�6Q   $��Center=�6l   ._�53   "   �� $��Center=�61   $��Center=�6L   #��z 6n   54      d� 56      $ PART_DropDownToggle��6+   ��.  8�� $��2,1,2,1q�57   �   $��3��58   *   # z 6;   #��z 59   +    57   *   �� *IsDropDownOpen+$ TwoWay=� �� *TemplatedParent+59   +   �� 5;      $��1��6%   ._�5<   "   �� $ Message��6,   $ 0q�6>   $y�	Collapsed=�6J   #��z 6a   #��z��5=   (    .  8 	 $
��Left=�6�   $ Center=�6�   $��Center=�5>   (   $��5,2,5,2q�6C   5?      � 5A      $��1��6#   .+�5B   "   �� $ PART_TextBox��6*   8��
 $��Stretch=�6n   # z 5C   &    .  . #��z��5D   &   #��z��6P   #��z��6z   ��5E   &   �� $ SelectedDate�$ OneWay=� �� *TemplatedParent+8  5F      � 5H      $ 
PART_Popup��6$    . $��Bottom=�6R     .  ! 5I   $   �� $" 	PART_Root��6V   5J      5K      # 5M      �� 5O      $$  ��6j   % 6&   ��  �� *Self+$
 Text�.a�5P      �� $& Message��6%    � y�$ Visible=�6P   5Q      P� 5R      ' �@J�IsMouseOver( . . �5S      ��  6<   �� $9 3(attach:ControlAttachProperty.MouseOverBorderBrush)� �� *Self+ � ��5T      P� 5U      ' �@J�	IsFocused( . . �5V      ��  6=   �� $5 /(attach:ControlAttachProperty.FocusBorderBrush)� �� *Self+ � ��5W      P� 5X      ' �@J�IsKeyboardFocusWithin( . . �5Y      ��  6=   �� $5 /(attach:ControlAttachProperty.FocusBorderBrush)� �� *Self+ � ��5Z      P� 5[      ' � {�( .  . �5\      �� $& 	PART_Root��6%   8   �@J�Opacity6O   5]      5_      5a      5b      �� 5e      "   6   8)   .�5f   
   ��  �@ AttachContent 5g      �� 5h      .R�5i        $	* ��6'   8�� ��.  $+ 0q�6x   , . $3��-attach:ControlAttachProperty.ClearTextCommand��5j   i   $��1,3,0,4q�5l       $- 14��61   8�� $. Arrow��6m   ��5k   $   ��  �� *FindAncestor+"/   5l   m   5m      5o      5p      �� 5s      "   6   8)   .�5t   
   ��  � ��$	 260��6"   60   �� 5u   
    �@ LabelTemplate 5v      �� 5w      " ��6"   .R�5x      �� $��60��6   8�� ._�5y      �� $��Center=�6$   $��Right=�6?   $��2q�6[   #��z��6f   6�   5z      5{      5}      5~      �� 5�      "   6   8)   .�5�   
   ��  � ��$	 280��6"   60   �� 5�   
    �@ LabelTemplate 5�      �� 5�      " ��6"   .R�5�      �� $��60��6   8�� ._�5�      �� $��Center=�6$   $��Right=�6?   $��2q�6[   #��z��6f   6�   5�      5�      5�      �� 5�   
    �@ AttachContent 5�      �� 5�      .R�5�        $	* ��6'   8�� ��.  $+ 0q�6x   , . $3��-attach:ControlAttachProperty.ClearTextCommand��5�   e   $��0,3,0,4q�5�       $- 14��61   8�� $. Arrow��6m   ��5�       ��  �� *FindAncestor+"/   5�   m   5�      5�      5�      5�      
   M S B A M L   `   `   `  ���� 	  Kurs2    Kurs2.View.ResultView   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     c _MaterialDesignThemes.MahApps, Version=5.2.1.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a_ [MaterialDesignThemes.Wpf, Version=5.2.1.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a<md1http://materialdesigninxaml.net/winfx/xaml/themes     � Title$  
ResultView��$	��500��$
��1200�� � Icon$ /f1.png��.��� .+�)  #MaterialDesignThemes.Wpf.DialogHost  -     Name$ LoginDialogHost��   
Identifier$ LoginDialog��.���  � RowDefinitions �� $
��Auto ��� .+��� -    ��Name$ NavigationPanel�� �� Orientation$ 
Horizontal=�$��Center=�$	��0,8q�.+��� -    ��Name$ DriversButton��$��Личный зачет��   MaterialDesignFlatButton#��[  $��4q�
 � Tag$) #/Pages/DriversChampionshipPage.xaml���� -   $#��Командный зачет��#��[  $��4q�$. (/Pages/ConstructorsChampionshipPage.xaml���� -   $ AddResultButton��$<��6Добавление результатов гонки��#��[  $��4q�$% /Pages/AddRacingResultPage.xaml��$y�	Collapsed=��� -   $ AddRaceButton��$��
Гонки��#��[  $��4q�$ /Pages/RacesPage.xaml��$y�	Collapsed=�	 � Cursor$
	 Hand���� -   $ AddSeasonButton��$'��!Добавление сезона��#��[  $��4q�$ /Pages/AddSeasonPage.xaml��$y�	Collapsed=��� -   $ DriverConstructorButton��$��Контракты��#��[  $��4q�$2 ,/Pages/DriverConstructorAffiliationPage.xaml��$y�	Collapsed=��� -	   $ DriverButton��$��Пилоты��#��[  $��4q�$ /Pages/DriversPage.xaml��$y�	Collapsed=��� -
   $ ConstructorButton��$��Конструкторы��#��[  $��4q�$" /Pages/ConstructorsPage.xaml��$y�	Collapsed=��� -   $ CircuitsButton��$��Трассы��#��[  $��4q�$ /Pages/CircuitsPage.xaml��$y�	Collapsed=� � -   
  �Name$
 	MainFrame��$��2��  � NavigationUIVisibility$ Hidden=��� -   $ LoginButton��$��
Войти��$��90��$��36��$��Right=�$��Center=�$��0,0,20,0q�   OpenDialogCommand#��Z $��0��   DialogContent � .+��� ���	#80000000�� 
������� �� CornerRadius$ 8�� �� Padding$ 24q�$	��350��$��Center=�$��Center=�._��� .+��� $ ��Вход в систему��$��20(�$
��Bold"�$��0,0,0,16q��� -    ��Name$ LoginTextBox��)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 
Логин��$��0,0,0,16q�� $��0,0,0,24q� � ColumnDefinitions �� �� $
��Auto �.+�G� -    G�Name$ PwdBox��$ Пароль���� -   $ 
PwdTextBox��$ Пароль��$y�	Collapsed=�d� -   $��1��$��Center=�$��8,0,0,0q� " MaterialDesignFlatToggleButton#���  .��'  !MaterialDesignThemes.Wpf.PackIcon     Kind$	 Eye=��� $ 
Horizontal=�$��Right=�.+��� $��Отмена��   CloseDialogCommand#��Z $��False���� -   $��
Войти��$
��True��$��8,0,0,0q��� -   $ ToastContainer��$��Center=�$��Bottom=�$��	0,0,0,150q�$ Vertical=� K� ZIndex$	 100��$��3��
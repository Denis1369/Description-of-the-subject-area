   M S B A M L   `   `   `  ����	  Kurs2.    (Kurs2.Pages.ConstructorsChampionshipPage   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        N� Title$"  ConstructorsChampionshipPage��5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ 	ExportBtn��5      $��Экспорт в PDF��5      $��10q�5      $��5q�5      $��Right=�5      5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ ResultsGrid��5      $��1��5         AutoGenerateColumns$ False��5         CanUserSortColumns$ False��5         
IsReadOnly$
 True��5         CanUserAddRows$ False��5         	RowHeight$ 35Q�5      	   Columns	 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn
   Header$
 Позиция��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 5   %   �� *[Позиция]+4  .System.Windows.Controls.DataGridTemplateColumn  5      $
 Конструктор��6)      CellTemplate 5      �� 5      .R�5       �� �� 5!   "   -   5        
���   � �� TextDecorations$
 None{�5%   #   ��5"   #   �� *[ConstructorUrl]+.�5%   :   .�5&   &   �� �6*   �� *[Конструктор]+5'   #   5(      5)      5+      5-      5.      5/      
   M S B A M L   `   `   `  ����  ExcelLoader    ExcelLoader.MainPage   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5	      � -   5          �Name$  
LayoutRoot��5	       � DataContext 6   �� .+�5
   
   �� $��2q�6   $��84��6%   $��39,34.989,58,0q�61   $	��Top=�6I    �� CornerRadius$ 5��6a   ��5      
� 5      .��5      �  � Color$ Black��6#    � Offset$ 0Q�61   � 5      $ 	#FF485CCC��6#   $ 1Q�65   � 5      $ 	#FF111632��6#   $ 0.246Q�65   � 5      $ 	#FF212B60��6#   $ 0.474Q�65   5      ._�5      �� $��导入excel文件：��6   $��6,6,0,0q�6)   $��27��6:   $	��Top=�6F   $
��Left=�6^   $	��100��6y   5      �� 5   
   -   5        $��	excelpath��5   {   $��52,72.989,204.513,0q�6    �� TextWrapping$
 Wrap=�6/   $��文件路径....��6C   $��26.837��6S   $	��Top=�6c   6{   �� 5   
   -   5        $��	文件...��5      $��Right=�6!   $��	0,68,96,0q�6=   $	��Top=�5      $��66��6)   $��32��64   6@   �� 5   
   $��2q�6   $��39,122.989,58,76.837q�6%   $ 5��6C   ��5      
� 5      .��5      � $ Black��6#   $ 0Q�61   � 5      $ 	#FF5461D4��6#   $ 1Q�65   � 5      $ 	#FF1A1E43��6#   $ 0.526Q�65   � 5      $ 	#FF3A4394��6#   $ 0.448Q�65   5      5!      �� 5"   
   -   5        $��sheetcmb��5"   E   $��153,0,240,152q�6   $��Bottom=�6*    �� DisplayMemberPath$ 	SheetName��6U   5#      �� 5$   
   $��	设置：��6   $
��Left=�6   $��54,133,0,181q�69   $��47��6O    J� RenderTransformOrigin$ 0.161,-2.944&�6Z   �� 5%   
   $��Sheet名称��6   $
��Left=�6"   $��54,175,0,152q�6=   $��70��6S   $ 0.161,-2.944&�6^   �� 5&   
   $��导入范围：��6   $
��Left=�6    $��
54,0,0,106q�6;   $��Bottom=�6O   $��70��6j   $��24��6u   $ 0.161,-2.944&�6�   �� 5'   
   -   5        $��Sr��5(   P   $
��Left=�5'      $��152,0,0,102q�6-   $
 Wrap=�6B   $��Bottom=�6V   $��51��5(      ���	#FF149522$
��Bold"�64   $��A2��6F   $��25��6Z   �� 5)   
   -   5        $��Er��5)   �   $��251,0,162,101q�6   $
 Wrap=�6)   $	��B10��6=   $��Bottom=�6H   ���	#FF22A12F$
��Bold"�6z   $��25��6�   � 5+   
   -   5        $��loadprogress��5,   ;   $��17��5+      $��52,0,170,14q�6"   $��Bottom=�67    � IsIndeterminate . ���	#FF09A126	 J� Opacity$		 0.8Q�5,   -   ��5-      �� 5.      
 �� EndPoint$	
 1,0&�6&    �� 
StartPoint$	 0,0&�65   .��5/      � $ 	#FFBABABA��6#   $ 0Q�65   � 50      $ 	#FFC7C7C7��6#   $	 0.5Q�65   � 51      $ 	#FF931141��6#   $ 1Q�65   52      54      �� 55   
   -   5        $��
Loadbutton��56   Z   $��开始导入��55      $��Right=�6    $��0,0,60,24.163q�6<   $��Bottom=�56      $��84��6,   $��27.837��67   6Z   �� 57   
   $��——>��6   $��207,0,0,102q�6   $��Bottom=�63   $
��Left=�6N   $��44��6i   $ 
0.545,1.28&�6t   	� 58   
   -	   5        $��检查井数据��58      $
��Left=�6&   $��
52,288,0,0q�6A   $	��Top=�6U   $ 0.544,1.571&�6�   	� 59   
   -
   5        $��管道数据��59      $
��Left=�6%   $��153,288,0,0q�6@   $	��Top=�6U   6m   	� 5:   
   -   5        $��内窥数据��5:      $
��Left=�6%   $��243,288,0,0q�6@   $	��Top=�6U   6m   5;      5<      
   M S B A M L   `   `   `  ����  PipeNetManager)    #PipeNetManager.pipeMsg.VideoControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      �   � RowDefinitions  5
   
   �� 5      $��30 �6   6*   �� 5      6   �� 5      $��25 �6   6*   �� 5      $��50 �6   6*   .+�5   
   � -   5        $��progress��5   4   $��0��6   $��0��6'    � Minimum$ 0Q�6D    � Maximum$ 1Q�6P    � Value$ .7Q�6\   $��16��6g   $y�Hidden=�6s    � IsIndeterminate . $��0,0,0,5q�5   2   6E   r� 5   
   -   5        $��Mediapalyer��5   4   $��0��6   $��1��6'   $��	10,3,10,0q�6G    r� LoadedBehavior$ Manual=�6Z   5      �� 5   
   -   5        $��
timeslider��5   :   $��0��6   $��2��6!   $��25��6.   $��	10,0,10,0q�6L   5   ;   �� 5   
   $��0��6   $��3��6#   $��10,0,0,0q�60   .+�5      �� -   5        $��Player��5   ,   $��33��6   $��70��6!   $��Bottom=�6:   $��0,0,0,-1q�6U   .��6|   播放6�   �� 5      -   5        $
��Stop��5   ,   $��33��6   $��70��6!   $��Bottom=�68   $��0,0,0,-1q�6S   .��6x   暂停6|   5       � Triggers 5   
   :� 5        :� RoutedEvent$ Button.Click��6    :� 
SourceName$ Player��66   [�5!      �� 5"      -   5        	 ��Name$	 myBegin��5"   &   .��5#      �� -   5        
 ��Name$
 
storyboard��5#   %   .��5$      p� -   5         g�Name$ timeline��5%   0    �� 
TargetName$ Mediapalyer��5$   ,    g� RepeatBehavior$ Forever��6P    g� 	BeginTime$ 0X�6i    g� Duration$ 5D�5%   @   5&      5'      5(      5*      :� 5+      $ Button.Click��6   $
 Stop��66   [�5,      �� 5-       �� BeginStoryboardName$ myBegin��6%   6E   5/      51      52      
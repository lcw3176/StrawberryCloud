   M S B A M L   `   `   `  ���� J  FStrawberryCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:StrawberryCloud.Views.HomeContent!StrawberryCloud.Views.HomeContent  3    -StrawberryCloud.Views.HomeContent.ProfileView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Alocal/clr-namespace:StrawberryCloud.Views.HomeContent       � 	Resources     MyButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers P� Property P� Value �� 
TargetName��%�   &	        �� " ��.���  �@�OverridesDefaultStyle . ��  � �� �� " ��.R��� $��border��#��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ��� $ border�� �@J�Opacity$	 0.7Q�.��� 
�������	 � RowDefinitions	 �� $��50 ��� $��* �.+�� $��0��
 � ColumnDefinitions
 �� $��50 ��� $��* ��� $��70 ��� $��70 ��� $��70 �.+��� $��0��$��5q�$��0q�#��[  ���� *fallbackCommand+����  �� ImageSource$ /Resource/Image/back.png�� j� Stretch$ UniformToFill=��� $��2��$��5q�$��0q�#��[  ���� *refreshCommand+���� $' !/Resource/Image/refreshButton.png��$ Uniform=��� $��3��$��5q�$��0q�#��[  ���� *uploadCommand+���� $&  /Resource/Image/uploadButton.png��$ Uniform=��� $��4��$��5q�$��0q�#��[  ���� *makeFolderCommand+���� $# /Resource/Image/newFolder.png��$ Uniform=��� 
���   �$��1q�$��1��$��10q�._��� $
��Auto=�$
��Auto=�.���� ���� *
fileList+ �� UpdateSourceTrigger$ PropertyChanged=����� .R�� $��30��$��10, 0q�
 �� $��* ��� $	��100 ��� $	��100 �.+��� $��0��$��Stretch=�$
��Left=�$��Center=�$��0q�
������ $��10, 0q����� *name+ J� InputBindings d�  d� Gesture$ LeftDoubleClicka� �� Command �� *clickCommand+ �� CommandParameter �� *name+�� $��1��$
��Left=�$��Center=����� *size+�� $��2��$
��Left=�$��Center=����� *type+ � ContextMenu �� .@�m� $��삭제�� m� Command �� *deleteCommand+ m� CommandParameter �� *name+ m� Icon �� $ ��/Resource/Image/delete.png��
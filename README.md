
# 文件和图片互转工具

这是一个文件和图片互转的小工具，有两种运行模式。项目由C#实现，支持多种压缩方式，支持文件加密，**只支持单文件操作**。

## 多种模式

- **图文界面模式**: 直接将文件转换成图片，图片大小会根据文件大小增加而增加。（超过100MB的图片可能会导致爆内存）
- **控制台模式**: 类似于图种的模式，程序会将图片和文件加密后数据拼合在一起生成新图片，允许编码超大文件。

## 主要功能

- **加密文件**: 允许加密文件，支持解密
- **编码图片**: 支持将文件转换成图片或者将图片转换回文件。
- **支持拖拽**: （v3.4）现在支持将文件拖拽到exe上进行操作。

## 使用说明

### 使用工具

- **图文界面模式**: 双击exe直接使用。
- **控制台模式**: 在exe所在的文件夹按下（shift+鼠标右键），点击“在此处打开 Powershell 窗口（S）”选项，具体操作方法请看下方控制台提示。
- **拖拽模式**: 将文件直接拖拽到exe上，然后按照弹窗提示进行操作。

### 控制台提示:
```
————————————————————————————————————————————
—　　　　　　　文件和图片互转工具　　　　　　　—
—　　　　　　　　　　　　　　　　　　　　　　　—
—　　　　　　　　　　　　　　　作者：绘梦璃　　—
————————————————————————————————————————————
————————————————————————————————————————————
[exeName].exe -[BTF/FTB/BTFW/FTBW] -IP [fileName] 
-S [partSize:1024/1k/10Mb/0.05G] -OP [outPath] 
-CM [No/CLZF/ZIP/Deflate] -NK -KM [MD5/SHA256]
-KV [You Password]

[exeName].exe -[BTF/FTB/BTFW/FTBW] -IP [fileName] 
-S [partSize:1024/1k/10Mb/0.05G] -OP [outPath] 
-CM [No/CLZF/ZIP/Deflate] -P [You Password]
————————————————————————————————————————————

-? -H -help 获取帮助
-FTB -fileToBmp 文件转为图片
-BTF -bmpToFile 图片转为文件
-IP -inputPath -inputpath 输入文件路径
-OP -outPath -outpath 输出文件路径(默认原路径)
　　　　　　　(使用此参数时,不会调用explorer)
-NK -needKey -needkey 需要密码(不输入:无)
-KM -keyMode -keymode 加密模式(默认:无)
-KV -keyValue -keyvalue 密码内容
-P -password 密码(等同于-NK -KM SHA256 -KV)
-CM -compressMode -compressmode 压缩模式(默认:无)
-S -size 启用分块,用于减少转化大文件时内存占用

————————————————————————————————————————————
v3.4新增内容：

现在本程序支持直接将文件拖拽到exe上进行操作了
请注意拖拽生成的文件名最好不要修改，否则会导致
程序无法解码
————————————————————————————————————————————

使用范例[方括号中为你应输入的内容](括号内为解释):
[exeName].exe -BTF -IP [fileName] -P [password]
将[fileName]以[password]加密为一张图片并保存

[exeName].exe -FTB -IP [fileName] -P [password]
将[fileName]以[password]解码为源文件并保存

-KM 加密模式:
-KM No(没有加密)/SHA256(sha256加密)

-CM 压缩模式(压缩不一定会让文件更小):
-CM No(没有压缩),CLZF/Deflate/ZIP(压缩模式)

————————————————————————————————————————————
!!　　　　　　　　　　　　　　　　　　　　　　!!
!!　　注意,使用-S必须使用-OP指定输出位置 　  !!
!!　　　　　　　　　　　　　　　　　　　　　　!!

使用范例[方括号中为你应输入的内容](括号内为解释):
[exeName].exe -BTF -IP [fileName] -S [partSize]
-OP [outPath] -CM [No/CLZF/ZIP/Deflate]
将[fileName]以每块[partSize]大小加密为一张图片并
以[No/CLZF/ZIP/Deflate]模式压缩并保存于[outPath]

-S  启用分块:(没有尾缀即为字节大小)
-S  1024(=1KB)/1KB/2MB/0.05GB(最大允许块:50MB)

————————————————————————————————————————————
```

### 下载
1. Clone 仓库(`git clone https://gitee.com/huimengli/FileToImage/ --depth 1`)
2. 最新版本下载地址 [文件和图片互转_v3.4](https://github.com/huimengli/FileToImage/releases/tag/%E6%96%87%E4%BB%B6%E5%92%8C%E5%9B%BE%E7%89%87%E4%BA%92%E8%BD%ACv3.4)

## 许可证

本项目采用MIT许可证。详情请查看仓库中的`LICENSE`文件。

## 作者

- [huimengli](https://github.com/huimengli)

感谢您使用此工具，如果有问题或者建议，请在Github上提交Issue。

# GITD-IEA
GITD-IEA is used to solve the problem that text information is difficult to detect under different scales and complex backgrounds, so as to improve the accuracy of text detection.
which is primarily based on
[EAST:An Efficient and Accurate Scene Text Detector](https://arxiv.org/abs/1704.03155v2),
[An end-to-end trainable neural network for image-based sequence recognition and its application to scene text recognition](https://arxiv.org/pdf/1507.05717v1),
This work combines deep learning with the improved East algorithm and builds a target detection model framework. 
The frame can adapt to different sizes of text, and accurately vectorize text in complex backgrounds.
The detection ability of the model is increased by 5.1% compared with the original model
The accuracy of the model is 4.5% higher than the original model
If this project is helpful to you, welcome to star.

# advantages
* The background algorithm uses Python language, Keras architecture.Use C# at the front desk,easy to learn and train
* base on EAST+CRNN, a frame for text vectorization under complex background
* easy to train the model
* Significant improvements have been made to more accurately locate 
  and recognize text in different colors and complex backgrounds.

In my experiment, 
the improved East obtained better detection accuracy than East, 
especially on text with complex background color in the picture.

# project files
## Text positioning part under complex background
* config file:
    cfg.py,control parameters
* pre-process data:
    preprocess.py,resize image
* label data:
    label.py,produce label info
* define network
    network.py
* define loss function
    losses.py
* execute training
    advanced_east.py and data_generator.py
* predict
    predict.py and nms.py

## The vectorized part of the text after positioning 
* main program :
  PicDemo.sln
* design Gaussian filter interface:
  Form1.cs
* design Image batch loading interface:
  Form2.cs
* The main interface of the front desk:
  MainForm.cs
* Conversion between pixels and latitude and longitude:
  GoogleCoordinate.cs
* Class definition in use:
  Class1.cs
* Image processing related operations:
  Filter.cs
* The main entry point for the application：
  Program.cs
* Create thumbnail:
  thumbnail.cs


# network arch
* Text positioning part under complex background

**网络输出说明：
输出层分别是1位score map, 是否在文本框内；2位vertex code，是否属于文本框边界像素以及是头还是尾；4位geo，是边界像素可以预测的2个顶点坐标。所有像素构成了文本框形状，然后只用边界像素去预测回归顶点坐标。边界像素定义为黄色和绿色框内部所有像素，是用所有的边界像素预测值的加权平均来预测头或尾的短边两端的两个顶点。头和尾部分边界像素分别预测2个顶点，最后得到4个顶点坐标。**

# setup
* C#
* python 3.6.3+
* tensorflow-gpu 1.5.0+(or tensorflow 1.5.0+)
* keras 2.1.4+
* numpy 1.14.1+
* tqdm 4.19.7+

# training
* tianchi ICPR dataset download
链接: https://pan.baidu.com/s/1NSyc-cHKV3IwDo6qojIrKA 密码: ye9y

* prepare training data:make data root dir(icpr),
copy images to root dir, and copy txts to root dir,
data format details could refer to 'ICPR MTWI 2018 挑战赛二：网络图像的文本检测',
[Link](https://tianchi.aliyun.com/competition/introduction.htm?spm=5176.100066.0.0.3bcad780oQ9Ce4&raceId=231651)
* modify config params in cfg.py, see default values.
* python preprocess.py, resize image to 256*256,384*384,512*512,640*640,736*736,
and train respectively could speed up training process.
* python label.py
* python advanced_east.py, train entrance
* python predict.py -p demo/001.png, to predict
* pretrain model download(use for test)
链接: https://pan.baidu.com/s/1KO7tR_MW767ggmbTjIJpuQ 密码: kpm2

# License
The codes are released under the MIT License.

# references
* [EAST:An Efficient and Accurate Scene Text Detector](https://arxiv.org/abs/1704.03155v2)

* [CTPN:Detecting Text in Natural Image with Connectionist Text Proposal Network](https://arxiv.org/abs/1609.03605)

* [An end-to-end trainable neural network for image-based sequence recognition and its application to scene text recognition](https://arxiv.org/pdf/1507.05717v1)

* [Deep Matching Prior Network: Toward Tighter Multi-oriented Text Detection](https://arxiv.org/abs/1703.01425)


**网络输出说明：
输出层分别是1位score map, 是否在文本框内；2位vertex code，是否属于文本框边界像素以及是头还是尾；4位geo，是边界像素可以预测的2个顶点坐标。所有像素构成了文本框形状，然后只用边界像素去预测回归顶点坐标。边界像素定义为黄色和绿色框内部所有像素，是用所有的边界像素预测值的加权平均来预测头或尾的短边两端的两个顶点。头和尾部分边界像素分别预测2个顶点，最后得到4个顶点坐标。**

[原理简介(含原理图)](https://huoyijie.cn/blog/9a37ea00-755f-11ea-98d3-6d733527e90f/play)

**后置处理过程说明参见
[后置处理(含原理图)](https://huoyijie.cn/blog/82c8e470-7562-11ea-98d3-6d733527e90f/play)**

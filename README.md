# GITD-IEA
GITD-IEA is used to solve the problem that text information is difficult to detect under different scales and complex backgrounds, so as to improve the accuracy of text detection.
which is primarily based on:

* [EAST:An Efficient and Accurate Scene Text Detector](https://arxiv.org/abs/1704.03155v2)
* [An end-to-end trainable neural network for image-based sequence recognition and its application to scene text recognition](https://arxiv.org/pdf/1507.05717v1)

This work combines deep learning with the improved East algorithm and builds a target detection model framework.   
The frame can adapt to different sizes of text, and accurately vectorize text in complex backgrounds.  
The detection ability of the model is increased by 5.1% compared with the original model.  
The accuracy of the model is 4.5% higher than the original model.  
If this project is helpful to you, welcome to star.

# Advantages
* The background algorithm uses Python language, Keras architecture.Use C# at the front desk,easy to learn and train
* base on EAST+CRNN, a frame for text vectorization under complex background
* easy to train the model
* Significant improvements have been made to more accurately locate 
  and recognize text in different colors and complex backgrounds.

In my experiment, 
the improved East obtained better detection accuracy than East, 
especially on text with complex background color in the picture.

# Rroject Files
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
* The main entry point for the application???
  Program.cs
* Create thumbnail:
  thumbnail.cs


# Network arch
* Text positioning part under complex background

**?????????????????????
??????????????????1???score map, ????????????????????????2???vertex code????????????????????????????????????????????????????????????4???geo?????????????????????????????????2????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????2????????????????????????4??????????????????**

# setup
* C#
* python 3.6.3+
* tensorflow-gpu 1.5.0+(or tensorflow 1.5.0+)
* keras 2.1.4+
* numpy 1.14.1+
* tqdm 4.19.7+

# Training
* tianchi ICPR dataset download
??????: https://pan.baidu.com/s/1NSyc-cHKV3IwDo6qojIrKA ??????: ye9y

* prepare training data:make data root dir(icpr),
copy images to root dir, and copy txts to root dir,
data format details could refer to 'ICPR MTWI 2018 ??????????????????????????????????????????
??????: [Link](https://tianchi.aliyun.com/competition/introduction.htm?spm=5176.100066.0.0.3bcad780oQ9Ce4&raceId=231651)
* modify config params in cfg.py, see default values.
* python preprocess.py, resize image to 256 * 256 , 384 * 384 , 512 * 512 , 640 * 640 , 736 * 736,
and train respectively could speed up training process.
* python label.py
* python advanced_east.py, train entrance
* python predict.py -p demo/001.png, to predict
* pretrain model download(use for test)
??????: https://pan.baidu.com/s/1KO7tR_MW767ggmbTjIJpuQ ??????: kpm2

# License
The codes are released under the MIT License.

# References
* [EAST:An Efficient and Accurate Scene Text Detector](https://arxiv.org/abs/1704.03155v2)

* [CTPN:Detecting Text in Natural Image with Connectionist Text Proposal Network](https://arxiv.org/abs/1609.03605)

* [An end-to-end trainable neural network for image-based sequence recognition and its application to scene text recognition](https://arxiv.org/pdf/1507.05717v1)

* [Deep Matching Prior Network: Toward Tighter Multi-oriented Text Detection](https://arxiv.org/abs/1703.01425)


**?????????????????????
??????????????????1???score map, ????????????????????????2???vertex code????????????????????????????????????????????????????????????4???geo?????????????????????????????????2????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????2????????????????????????4??????????????????**

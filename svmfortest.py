import numpy as np
from sklearn.svm import SVC
from sklearn import svm
from sklearn.model_selection import train_test_split
from sklearn.metrics import f1_score
from sklearn.metrics import recall_score
from sklearn.metrics import classification_report, confusion_matrix
import pandas as pd
def predict_and_test(trainfile, testfile):
    traindata=pd.read_csv(trainfile)
    testdata=pd.read_csv(testfile)
    svclassifier = SVC(kernel='rbf')
    
    x_train =traindata.iloc[:,1:-1].values
    y_train =traindata.iloc[:,-1].values
    x_test = testdata.iloc[:,1:-1].values
    
    svclassifier.fit(x_train, y_train)
    y_pred = svclassifier.predict(x_test)
    output=pd.DataFrame(data={"id":testdata[:,1].values,"Prediction":y_pred, "Actual Result": testdata.iloc[:,-1].values}).to_csv('prediction.csv')
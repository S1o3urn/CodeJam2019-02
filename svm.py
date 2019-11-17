import numpy as np
from sklearn.svm import SVC
from sklearn import svm
from sklearn.model_selection import train_test_split
from sklearn.metrics import f1_score
from sklearn.metrics import recall_score
from sklearn.metrics import classification_report, confusion_matrix
import pandas as pd
def predict_and_test(filename):
    data=pd.read_csv(filename)
    data =data.sample(frac=1)
    svclassifier = SVC(kernel='rbf')
    
    x_all =data.iloc[:,1:-1].values
    y_all =data.iloc[:,-1].values
    x_train, x_test, y_train, y_test = train_test_split(x_all, y_all, test_size=0.2, random_state=0)
   
    svclassifier.fit(x_train, y_train)
    y_pred = svclassifier.predict(x_test)

    print(classification_report(y_test,y_pred))


if __name__ == "__main__":
   
    predict_and_test("allData.csv")
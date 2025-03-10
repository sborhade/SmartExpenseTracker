from flask import Flask, request, jsonify
import pandas as pd
from sklearn.linear_model import LinearRegression
import numpy as np

app = Flask(__name__)

@app.route('/forecast', methods=['POST'])
def forecast():
    data = request.get_json()
    dates = data['dates']  # List of historical dates
    expenses = data['expenses']  # List of historical expenses
    
    # Prepare data for forecasting
    df = pd.DataFrame({
        'ds': pd.to_datetime(dates),
        'y': expenses
    })
    df['ds'] = pd.to_datetime(df['ds'])
    
    # Linear regression for forecasting
    model = LinearRegression()
    df['month_num'] = np.arange(1, len(df) + 1)
    model.fit(df[['month_num']], df['y'])
    
    # Predict next month's expense
    next_month = len(df) + 1
    predicted_expense = model.predict([[next_month]])[0]
    
    return jsonify({"forecasted_expense": predicted_expense})

if __name__ == '__main__':
    app.run(debug=True)

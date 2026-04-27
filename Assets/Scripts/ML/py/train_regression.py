import sys
import json
import pandas as pd

from sklearn.model_selection import train_test_split
from sklearn.linear_model import LinearRegression
from sklearn.metrics import mean_absolute_error, r2_score


def main():
    if len(sys.argv) < 3:
        print("Usage: python train_regression.py <input_csv> <output_json>")
        return

    input_csv = sys.argv[1]
    output_json = sys.argv[2]

    df = pd.read_csv(input_csv)

    feature_columns = [
        "RoomCount",
        "WalkableTiles",
        "OptimalPath",
        "Complexity"
    ]

    target_column = "Efficiency"

    X = df[feature_columns]
    y = df[target_column]

    if len(df) < 5:
        raise ValueError("Not enough data to train model.")

    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.25, random_state=1701
    )

    model = LinearRegression()
    model.fit(X_train, y_train)

    predictions = model.predict(X_test)

    results = {
        "model": "LinearRegression",
        "totalRows": int(len(df)),
        "trainRows": int(len(X_train)),
        "testRows": int(len(X_test)),
        "meanAbsoluteError": float(mean_absolute_error(y_test, predictions)),
        "r2Score": float(r2_score(y_test, predictions)),
        "coefficients": {
            col: float(coef)
            for col, coef in zip(feature_columns, model.coef_)
        },
        "intercept": float(model.intercept_)
    }

    with open(output_json, "w") as f:
        json.dump(results, f, indent=4)

    print("Regression complete.")
    print(json.dumps(results, indent=4))


if __name__ == "__main__":
    main()
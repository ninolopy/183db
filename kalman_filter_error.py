import matplotlib.pyplot as plt
import os

def read_data():
    true = []
    predicted = []
    
    with open('MyFirstGame/Assets/logs/state.txt') as f:
        for i, line in enumerate(f):
            lst = line.split(' ')
            float_lst = [float(x) for x in lst]
            if i % 2 == 0:
                true.append(float_lst)
            else:
                predicted.append(float_lst)
    return (true, predicted)

def calculate_state_error(true, predicted):
    theta_error = []
    phi_error = []
    theta_dot_error = []
    phi_dot_error = []
    for i in range(len(true)):
        theta_error.append(abs(predicted[i][0] - true[i][0]) / true[i][0])
        phi_error.append(abs(predicted[i][1] - true[i][1]) / true[i][1])
        theta_dot_error.append(abs(predicted[i][2] - true[i][2]) / true[i][2])
        phi_dot_error.append(abs(predicted[i][3] - true[i][3]) / true[i][3])
    return (theta_error, phi_error, theta_dot_error, phi_dot_error)

def main():
    true, predicted = read_data()
    print(len(true))
    print(len(predicted))
    errors = calculate_state_error(true, predicted)
    for error_list in errors:
        plt.plot(error_list)
        plt.show()
        plt.figure()

if __name__ == '__main__':
    main()
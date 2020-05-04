import matplotlib.pyplot as plt

x0 = []
x1 = []
xhat0 = []
xhat1 = []

with open('MyFirstGame/Assets/logs/kalman.txt') as f:
	for line in f:
		lst = line.split(' ')
		x0.append(float(lst[0]))
		x1.append(float(lst[1]))
		xhat0.append(float(lst[2]))
		xhat1.append(float(lst[3]))
	plt.plot(x0[:1000], label='True Theta')
	plt.plot(x1[:1000], label='True Phi')
	plt.plot(xhat0[:1000], label='Theta Estimate')
	plt.plot(xhat1[:1000], label='Phi Estimate')
	plt.legend()
	plt.show()		

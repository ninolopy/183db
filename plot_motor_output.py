import matplotlib.pyplot as plt

internal_current = []
internal_omega = []

with open('MyFirstGame/Assets/logs/motor_output.txt') as f:
	for line in f:
		lst = line.split(' ')
		internal_current.append(float(lst[0]))
		internal_omega.append(float(lst[1]))
	plt.plot(internal_current[:1000], label='Internal Current')
	plt.plot(internal_omega[:1000], label='Internal Omega')
	plt.legend()
	plt.show()		

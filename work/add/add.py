import tensorflow.keras

x = tensorflow.keras.layers.Input(shape=(1,))
y = tensorflow.keras.layers.Input(shape=(1,))

z = tensorflow.keras.layers.add([x, y])
model = tensorflow.keras.models.Model(inputs=[x, y], outputs=z)

model.summary()
model.save("add.h5")

json_str = model.to_json()

with open("add.json", 'w') as fp:
    fp.write(json_str)





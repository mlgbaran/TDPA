from PIL import Image
import tkinter as tk
from tkinter import filedialog
import os
import sys


def CreateColorData(pixels):
    color_data = {}

    for y in range(0, 32):  # scanning the first 32*32 area for the color mapping
        for x in range(0, 32):

            r, g, b, a = pixels[x, y]

            if a == 0:
                continue
            else:
                if (r, g, b, a) in color_data:
                    print(
                        "There is a pixel with the same color, coordinates = x={0},y={1}".format(x, y))
                    
                # key is map, value is the actual color
                # the actual color value is 32 pixels to the right of the map-color value
                color_data[(r, g, b, a)] = pixels[x+32, y]

    return color_data


def ApplyMap(color_data, pixels,image_copy):
    for y in range(32, image_copy.height):
        for x in range(0, image_copy.width):
            r, g, b, a = pixels[x, y]
            if (r, g, b, a) != (0, 0, 0, 0):
                if (r, g, b, a) in color_data:
                    pixels[x, y] = color_data[(r, g, b, a)]
                else:
                    print("This pixel is not in the keys: {0}".format(
                        pixels[x, y]))
                    
def GetFile():
    root = tk.Tk()
    root.withdraw()  # Hide the root window

    # Open a file dialog for opening a file
    file_path = filedialog.askopenfilename()

    # Check if a file was selected
    if file_path:
        print(f'Selected file: {file_path}')
    else:
        print('No file selected')
        sys.exit()
    return file_path


if __name__ == "__main__":

    # Create a Tkinter root window (if not already created)
    
    file_path = GetFile()

    file_name = os.path.basename(file_path)[:-4]

    image = Image.open(file_path)

    file_dir = os.path.dirname(file_path)

    savefilename = input("Save file as: ")

    print("Saving file: " + file_dir  + "/" + savefilename + ".png")

    image_copy = image

    pixels = image_copy.load()

    color_data = CreateColorData(pixels)

    ApplyMap(color_data, pixels,image_copy)

    image_copy.save(file_dir + "/" + savefilename + ".png")

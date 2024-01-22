from PIL import Image
import tkinter as tk
from tkinter import filedialog
import os
import sys


def TransformColor(pixels):

    for y in range(0, 32):  # scanning the first 32*32 area for the color mapping
        for x in range(0, 32):
            r, g, b, a = pixels[x, y]

            if a == 0:
                continue
            else:
                pixels[x+32, y] = x, 31-y, 0, 255

    return


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

    print("Saving file: " + file_dir + "/" + savefilename + ".png")

    image_copy = image

    pixels = image_copy.load()

    TransformColor(pixels)

    image_copy.save(file_dir + "/" + savefilename + ".png")

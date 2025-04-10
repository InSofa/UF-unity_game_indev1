import tkinter as tk
import numpy as np
from perlin_noise import PerlinNoise
from PIL import Image, ImageTk

# Parameters
WIDTH, HEIGHT = 500, 500  # Canvas size
NOISE_SCALE = 10.0        # Perlin noise scale (if generating)
FREQ = 10                 # Base frequency of decorations
NOISE_THRESHOLD = 0.3     # Minimum noise value to place decorations
PERLIN_IMAGE_PATH = "C:/users/simonkalmi.claesson/downloads/perlin-2d-grey.jpg"  # Set to "perlin_noise.png" or other filename to use an image

# Define no-go zones (left_x, top_y, right_x, bottom_y)
NO_NO_SQUARES = [
    (100, 100, 200, 200),
    (300, 300, 400, 400)
]

# Load a Perlin noise image or generate one
def get_noise_map(width, height, scale, image_path=None):
    if image_path:
        img = Image.open(image_path).convert("L").resize((width, height))
        noise_map = np.array(img) / 255.0  # Normalize to 0-1
        return noise_map, img
    else:
        noise = PerlinNoise(octaves=4, seed=42)
        noise_map = np.zeros((width, height))
        for x in range(width):
            for y in range(height):
                noise_value = noise([x / scale, y / scale])
                noise_map[x, y] = (noise_value + 1) / 2  # Normalize to 0-1
        img = Image.fromarray((noise_map * 255).astype(np.uint8))
        return noise_map, img

# Check if a point is inside a no-go zone
def is_in_no_no_zone(x, y, no_no_squares):
    for (lx, ty, rx, by) in no_no_squares:
        if lx <= x <= rx and ty <= y <= by:
            return True
    return False

# Generate decoration points based on Perlin noise
def generate_decorations(freq, noise_map, width, height, threshold):
    decorations = []
    grid_size = int(np.sqrt(freq * 10))  # Adaptive grid

    for x in range(grid_size):
        for y in range(grid_size):
            sample_x = int(x / grid_size * width)
            sample_y = int(y / grid_size * height)

            noise_value = noise_map[sample_x, sample_y]
            if noise_value > threshold:
                local_freq = int(freq * noise_value)  # Scale by noise
                for _ in range(local_freq):
                    cand_x = np.random.randint(sample_x, sample_x + width // grid_size)
                    cand_y = np.random.randint(sample_y, sample_y + height // grid_size)

                    if not is_in_no_no_zone(cand_x, cand_y, NO_NO_SQUARES):
                        decorations.append((cand_x, cand_y))

    return decorations

# Main GUI
class PerlinVisualizer(tk.Tk):
    def __init__(self, noise_image_path=None):
        super().__init__()
        self.title("Perlin Noise Decoration Placement")

        # Get noise map (either from image or generated)
        self.noise_map, self.noise_img = get_noise_map(WIDTH, HEIGHT, NOISE_SCALE, noise_image_path)
        self.decorations = generate_decorations(FREQ, self.noise_map, WIDTH, HEIGHT, NOISE_THRESHOLD)

        # Convert noise image to Tkinter format
        self.noise_img = ImageTk.PhotoImage(self.noise_img)

        # Create canvas
        self.canvas = tk.Canvas(self, width=WIDTH, height=HEIGHT)
        self.canvas.pack()

        # Draw noise background
        self.canvas.create_image(0, 0, anchor=tk.NW, image=self.noise_img)

        # Draw no-go zones
        for (lx, ty, rx, by) in NO_NO_SQUARES:
            self.canvas.create_rectangle(lx, ty, rx, by, fill="black", outline="black")

        # Draw decorations as red dots
        for x, y in self.decorations:
            self.canvas.create_oval(x-2, y-2, x+2, y+2, fill="red", outline="red")

# Run GUI
if __name__ == "__main__":
    app = PerlinVisualizer(PERLIN_IMAGE_PATH)  # Pass an image path or None
    app.mainloop()

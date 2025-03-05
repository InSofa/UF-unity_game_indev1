import random
import tkinter as tk
import math

# Function to generate decoration coordinates
def generate_deco_coords(freq, outer_bounds, no_no_squares, min_dist=None, max_validations=50):
    valid_points = []
    weighted_candidates = []
    attempts = freq * 2  # Generate more than needed

    for _ in range(attempts):
        candidate = (
            random.uniform(outer_bounds[0], outer_bounds[2]),  # X between left_x and right_x
            random.uniform(outer_bounds[1], outer_bounds[3])   # Y between top_y and bottom_y
        )

        if is_in_no_no_zone(candidate, no_no_squares):
            continue  # Skip if inside a no-no zone

        weight = 1.0  # Default weight if no minDist

        if min_dist:
            nearest_dist = get_nearest_distance(candidate, valid_points)
            if nearest_dist < min_dist:
                weight = nearest_dist / min_dist  # Normalize weight

        if not min_dist or weight >= 1.0:
            valid_points.append(candidate)
            if len(valid_points) >= freq:
                break
        else:
            weighted_candidates.append((candidate, weight))

    # If we didn't reach freq, take highest-weighted near-valid points
    if len(valid_points) < freq:
        weighted_candidates.sort(key=lambda x: x[1], reverse=True)  # Sort by weight (highest first)
        valid_points.extend([p[0] for p in weighted_candidates[:freq - len(valid_points)]])

    return valid_points

# Check if a point is in a no-no zone
def is_in_no_no_zone(point, no_no_squares):
    x, y = point
    for rect in no_no_squares:
        if rect[0] <= x <= rect[2] and rect[1] <= y <= rect[3]:
            return True
    return False

# Get distance to the nearest existing point
def get_nearest_distance(point, others):
    x1, y1 = point
    min_dist = float("inf")
    for x2, y2 in others:
        dist = math.dist((x1, y1), (x2, y2))
        if dist < min_dist:
            min_dist = dist
    return min_dist

# Render points using Tkinter
def render_points(points, outer_bounds, no_no_squares):
    canvas_size = 500  # Window size
    scale_x = canvas_size / (outer_bounds[2] - outer_bounds[0])
    scale_y = canvas_size / (outer_bounds[3] - outer_bounds[1])

    root = tk.Tk()
    root.title("Decoration Placement")
    canvas = tk.Canvas(root, width=canvas_size, height=canvas_size, bg="white")
    canvas.pack()

    # Draw no-no zones
    for rect in no_no_squares:
        x1, y1 = (rect[0] - outer_bounds[0]) * scale_x, (rect[1] - outer_bounds[1]) * scale_y
        x2, y2 = (rect[2] - outer_bounds[0]) * scale_x, (rect[3] - outer_bounds[1]) * scale_y
        canvas.create_rectangle(x1, y1, x2, y2, fill="gray", outline="black")

    # Draw points
    for x, y in points:
        scaled_x = (x - outer_bounds[0]) * scale_x
        scaled_y = (y - outer_bounds[1]) * scale_y
        canvas.create_oval(scaled_x - 3, scaled_y - 3, scaled_x + 3, scaled_y + 3, fill="red", outline="black")

    root.mainloop()

# Example usage
outer_bounds = (0, 0, 100, 100)  # (left_x, top_y, right_x, bottom_y)
no_no_squares = [(30, 30, 50, 50), (70, 10, 90, 40)]  # List of (left_x, top_y, right_x, bottom_y)
points = generate_deco_coords(freq=100, outer_bounds=outer_bounds, no_no_squares=no_no_squares, min_dist=5, max_validations=50)

# Render result
render_points(points, outer_bounds, no_no_squares)

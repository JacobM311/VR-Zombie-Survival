# Physics Interactions in Unity

## Project Overview

The goal of this project is to develop realistic and responsive physics-based interactions in Unity without the use of Unity's interaction system. This involves implementing various types of interactions using Unity's physics system to create an immersive experience for users. By leveraging Rigidbody components, I aim to achieve smooth and natural movements that respond accurately to user inputs and environmental factors.

## Goals

1. **Create Physics-Based Interactions**: Implement various types of physics interactions to enhance the realism and responsiveness of objects in the Unity environment.
2. **Develop from Scratch**: Build these interactions from the ground up, without relying on pre-made solutions, to gain a deeper understanding of Unity's physics system and customize the interactions to fit specific needs.

## Physics Interactions Implemented

### 1. Smooth Object Grabbing

#### Description:
Objects can be grabbed by the user and will smoothly move to the desired position, allowing for a natural and immersive interaction.

#### Key Features:
- **Velocity-Based Movement**: Objects move using calculated velocities to ensure smooth and realistic transitions.
- **Offset Handling**: Correctly accounts for offsets when grabbing, ensuring the object appears to be grabbed from the user's perspective.
- **Parenting Compatibility**: Works seamlessly whether the object is parented to another object or not.

### 2. Lagged Following

#### Description:
Objects follow a target position with a slight delay, simulating a realistic weight effect that can be used for various applications, such as simulating weight when objects are grabbed.

#### Key Features:
- **Position and Rotation Interpolation**: Uses linear interpolation (Lerp) for position and spherical interpolation (Slerp) for rotation to achieve smooth transitions.
- **Physics Integration**: Updates the Rigidbody's velocity and angular velocity to ensure physically accurate movements.
- **Parent-Child Relationship Handling**: Adjusts calculations based on whether the object has a parent, maintaining consistent behavior.

### 3. Ease-In Movement

#### Description:
Objects ease into their new positions and rotations over a specified period, creating smooth and gradual transitions that enhance the realism of interactions.

#### Key Features:
- **Easing Functionality**: Gradually increases the speed of movement to create a smooth transition effect.
- **Dynamic Adjustment**: Continuously updates the target position and rotation based on user inputs and environmental changes.
- **Robust to Changes**: Ensures that the easing effect remains consistent even when the object is parented to another object.

### Usage

1. **Direct Interactor**:
    - Attach the `JVRDirectInteractor` script to any GameObject that you want to use for physical interacrions, this object will automatically populate with the needed components and values once the script is added.
    - Adjust `positionLerpSpeed` and `rotationLerpSpeed` to control the lag effect.

2. **Grab Interactable**:
    - Attach the `JVRGrabInteractable` script to any GameObject that you want to be interactable with smooth grabbing functionality.
    - Set grabbable objects to the correct layer for the interactor to use.
    - Set the appropriate hand attach transforms and configure the lerp speeds and easing times as needed.

### To Do:
- **Update Physics Scripts**: Update scripts I've made for throwing physics using the Unity interaction system to work with my custom interactions.
- Make a modular system that can be easily applied to different object to apply physics interaction within the editor, goal will be to eventually move this to the Asset Store.

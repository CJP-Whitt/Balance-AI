# Balance-AI
ML-Agents implementation for balancing single wheeled vehicles.

### **Goal**: 
Create a reinforcement model for balancing Single Wheeled vehicles.
- Inputs: Variations of vehicle state observations [power, rpm, accel_XYZ, gyro_XYZ, ]
- Output: Power (current or duty cycle)

### **Abstract:**
Balance, in this case, is defined as “parallel to the ground, with regard to the standing platform of the vehicle”. In the case of a ‘OneWheel’ and other single-wheeled balancing vehicles (SWBV), this “parallel to the ground” nature is important for riding on all types of terrains in order to maximize maneuverability while avoiding contact with the ground. The goal of using reinforcement learning in this project is to teach a single-wheeled styled agent to keep the distance to the ground of all four corners of the standing platform as equal as possible. It also key that the results are easily tuneable in order to enable different riding styles.

### **Methods:**
- Use Unity engine with ML-Agents package.
- Create single-wheeled electric vehicle style geometry  to teach the SWBV agent to balance in various environments.
- Train with traversing multiple environment terrains such as.
    - Flat 
    - Sinusoidal Hills 
    - Resistive Terrain
    - Cambered Hills

----
## Machine Learning
### **Approaches:**
1. Vision vs No Vision (# of frames per observation)
    - *Vision* is the process of using a window of past data as a observation for the model.
    - Vision is more computationally intensive for training and testing but it can improve pattern recognition, and in turn predictions by the model.
1. Dynamic vs Static (reactability)
    - A *Dynamic* model would ideally be able to adjust outputs despite unseens outside forces.
        - Training will be more complex and take longer since more parameters must be randomized after each iteration to ensure dynamic learning.
        - This will make most tuneablity nearly impossible since the model will be able to adjust dynamically to any static adjustments.
    - A *Static* model would ideally map inputs to outputs, resulting in little reactability to new stimulus.
        - Training will be simple, and enable a solid base for balancing with the ability to add on later.
        - Tuneability will be easy since the model won't dyncamically adjust to unknown stimuli. You may even beable to use customizable power curves.

### **Specifics:**
#### *Scenarios*


----
## Status

### **Progress:**

### **Issues:**

### **Future Plans:**


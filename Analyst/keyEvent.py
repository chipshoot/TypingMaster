import datetime

class KeyEvent:
    key: str  # The expected key
    typed_key: str  # The actual key typed
    is_correct: bool  # Whether the key was typed correctly
    key_down_time: datetime  # When the key was pressed
    key_up_time: datetime  # When the key was released
    latency: float  # Time between key press and release
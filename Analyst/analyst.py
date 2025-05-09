"""Module for analyzing typing drill statistics from the database."""
import json
from datetime import datetime
import pandas as pd
import psycopg2
from psycopg2.extras import RealDictCursor

def connect_to_db():
    """Connect to the database."""
    conn = psycopg2.connect(
        dbname="TypingMaster",
        user="postgres",
        password="Password1!",
        host="localhost",
        port="5432"
    )
    return conn

def get_drill_stats():
    """Get the drill stats from the database."""
    conn = connect_to_db()
    cur = conn.cursor(cursor_factory=RealDictCursor)
    
    # Query to get drill stats with key events
    query = """
    SELECT id, course_id, lesson_id, key_events_json, wpm, accuracy, 
           start_time, finish_time, practice_text, typed_text
    FROM drill_stats
    """
    
    cur.execute(query)
    results = cur.fetchall()
    cur.close()
    conn.close()
    return results

def analyze_key_events(drill_stats):
    """Analyze the key events data from the drill stats."""
    # Create a list to store all key events
    all_events = []
    
    for stat in drill_stats:

        # Parse the key_events_json field
        key_events = stat['key_events_json']
        
        for event in key_events:
            event_data = {
                'drill_id': stat['id'],
                'course_id': stat['course_id'],
                'lesson_id': stat['lesson_id'],
                'key': event['key'],
                'typed_key': event['typedKey'],
                'is_correct': event['isCorrect'],
                'key_down_time': datetime.fromisoformat(event['keyDownTime'].replace('Z', '+00:00')),
                'key_up_time': datetime.fromisoformat(event['keyUpTime'].replace('Z', '+00:00')),
                'latency': event['latency'],
                'wpm': stat['wpm'],
                'accuracy': stat['accuracy'],
                'practice_text': stat['practice_text'],
                'typed_text': stat['typed_text']
            }
            all_events.append(event_data)
    
    # Convert to DataFrame for analysis
    df = pd.DataFrame(all_events)
    return df

def generate_analysis(df):
    """Generate and print analysis of the key events data."""
    # Basic statistics
    print("\n=== Basic Statistics ===")
    print(f"Total number of key events: {len(df)}")
    print(f"Average accuracy: {df['accuracy'].mean():.2f}%")
    print(f"Average WPM: {df['wpm'].mean():.2f}")
    
    # Key-specific analysis
    print("\n=== Key-specific Analysis ===")
    key_stats = df.groupby('key').agg({
        'is_correct': ['count', 'mean'],
        'latency': 'mean'
    }).round(2)
    key_stats.columns = ['total_presses', 'accuracy', 'avg_latency']
    print(key_stats)
    
    # Time-based analysis
    print("\n=== Time-based Analysis ===")
    df['duration'] = (df['key_up_time'] - df['key_down_time']).dt.total_seconds()
    time_stats = df.groupby('drill_id').agg({
        'duration': ['mean', 'std'],
        'wpm': 'mean',
        'accuracy': 'mean'
    }).round(2)
    time_stats.columns = ['avg_duration', 'duration_std', 'avg_wpm', 'avg_accuracy']
    print(time_stats)
    
    # Error analysis
    print("\n=== Error Analysis ===")
    errors = df[df['is_correct'] == False]
    error_patterns = errors.groupby(['key', 'typed_key']).size().sort_values(ascending=False)
    print("Most common error patterns:")
    print(error_patterns.head(10))

def main():
    """Main function to execute the typing drill analysis."""
    # Get data from database
    drill_stats = get_drill_stats()
    
    # Convert to DataFrame
    df = analyze_key_events(drill_stats)
    
    # Generate analysis
    generate_analysis(df)
    
    # Save to CSV for further analysis
    df.to_csv('key_events_analysis.csv', index=False)
    print("\nData has been saved to 'key_events_analysis.csv'")

if __name__ == "__main__":
    main()

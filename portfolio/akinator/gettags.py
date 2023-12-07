import csv

# Read the CSV file
with open('footballers.csv', 'r', encoding='utf-8') as f:
    reader = csv.DictReader(f)
    tags = []
    for row in reader:
        # Get the 'player_tags' column and split it into separate tags
        tags.extend(row['player_tags'].split(','))

# Remove leading/trailing whitespace and '#' character
tags = [tag.strip().strip('#') for tag in tags]

# Convert the list to a set to remove duplicates, then convert it back to a list
distinct_tags = list(set(tags))

# Print the distinct tags
for tag in distinct_tags:
    print(tag)
print(len(distinct_tags))
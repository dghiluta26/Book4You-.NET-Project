INSERT INTO Accomodations
(Title, Location, Address, Description, PricePerNight, Capacity, Bedrooms, Bathrooms, Type, ImageUrl, Rating, IsAvailable)
VALUES
('Ocean View Villa', 'Santorini, Greece', 'Cliffside Road 12',
'Luxury villa with amazing sea view and private terrace.',
320.00, 4, 2, 2, 'Villa', '/images/stays/villa1.jpg', 4.9, 1),

('Cozy Mountain Cabin', 'Brasov, Romania', 'Forest Road 8',
'Warm wooden cabin near the mountains, perfect for winter stays.',
150.00, 3, 2, 1, 'Cabin', '/images/stays/cabin1.jpg', 4.7, 1),

('Modern City Apartment', 'Paris, France', 'Central Street 21',
'Modern apartment close to the city center and public transport.',
210.00, 2, 1, 1, 'Apartment', '/images/stays/apartment1.jpg', 4.5, 1),

('Beachfront Studio', 'Maldives', 'Island Beach 5',
'Studio right on the beach with stunning ocean views.',
400.00, 2, 1, 1, 'Studio', '/images/stays/studio1.jpg', 4.8, 1);

EXEC sp_rename 'Accomodations', 'Accommodations';

SELECT name
FROM sys.tables
ORDER BY name;

INSERT INTO Accommodations
(Title, Location, Address, Description, PricePerNight, Capacity, Bedrooms, Bathrooms, Type, ImageUrl, Rating, IsAvailable, CreatedAt)
VALUES
('Ocean View Villa', 'Santorini, Greece', 'Cliffside Road 12',
'Luxury villa with amazing sea view and private terrace.',
320.00, 4, 2, 2, 'Villa', '/images/stays/villa1.jpg', 4.9, 1, GETDATE()),

('Cozy Mountain Cabin', 'Brasov, Romania', 'Forest Road 8',
'Warm wooden cabin near the mountains, perfect for winter stays.',
150.00, 3, 2, 1, 'Cabin', '/images/stays/cabin1.jpg', 4.7, 1, GETDATE()),

('Modern City Apartment', 'Paris, France', 'Central Street 21',
'Modern apartment close to the city center and public transport.',
210.00, 2, 1, 1, 'Apartment', '/images/stays/apartment1.jpg', 4.5, 1, GETDATE()),

('Beachfront Studio', 'Maldives', 'Island Beach 5',
'Studio right on the beach with stunning ocean views.',
400.00, 2, 1, 1, 'Studio', '/images/stays/studio1.jpg', 4.8, 1, GETDATE());

SELECT * FROM Accommodations

UPDATE Accommodations
SET 
    Title = 'Santorini Cliffside Villa',
    Location = 'Santorini, Greece',
    Description = 'Luxury villa overlooking the Aegean Sea with breathtaking sunset views.'
WHERE Id = 3;

SELECT * FROM Accommodations
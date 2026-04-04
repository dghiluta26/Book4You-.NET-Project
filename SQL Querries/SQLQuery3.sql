SELECT * FROM Accommodations

INSERT INTO Accommodations
(Title, Location, Address, Description, PricePerNight, Capacity, Bedrooms, Bathrooms, Type, ImageUrl, Rating, IsAvailable, CreatedAt)
VALUES

-- 🇬🇷 GREECE
('Santorini Sunset Villa', 'Santorini, Greece', 'Oia Cliff 7',
'Luxury villa with panoramic views over the Aegean Sea and iconic sunsets.',
350, 4, 2, 2, 'Villa', '/images/stays/villa2.jpg', 4.9, 1, GETDATE()),

-- 🇮🇹 ITALY
('Lake Como Luxury Apartment', 'Como, Italy', 'Via Roma 12',
'Modern apartment overlooking Lake Como with a private balcony.',
280, 3, 1, 1, 'Apartment', '/images/stays/apartment2.jpg', 4.8, 1, GETDATE()),

-- 🇫🇷 FRANCE
('Paris Boutique Studio', 'Paris, France', 'Rue de Rivoli 45',
'Charming studio in central Paris, close to museums and cafes.',
190, 2, 1, 1, 'Studio', '/images/stays/studio2.jpg', 4.6, 1, GETDATE()),

-- 🇨🇭 SWITZERLAND
('Alpine Mountain Cabin', 'Zermatt, Switzerland', 'Alpine Road 3',
'Wooden cabin with direct view of the Matterhorn and ski access.',
220, 4, 2, 1, 'Cabin', '/images/stays/cabin2.jpg', 4.9, 1, GETDATE()),

-- 🇪🇸 SPAIN
('Barcelona Beach Apartment', 'Barcelona, Spain', 'La Rambla 88',
'Bright apartment near the beach with modern design.',
210, 3, 2, 1, 'Apartment', '/images/stays/apartment3.jpg', 4.5, 1, GETDATE()),

-- 🇩🇪 GERMANY
('Berlin City Loft', 'Berlin, Germany', 'Alexanderplatz 10',
'Spacious loft in the heart of Berlin nightlife.',
170, 2, 1, 1, 'Apartment', '/images/stays/apartment4.jpg', 4.4, 1, GETDATE()),

-- 🇷🇴 ROMANIA
('Carpathian Forest Cabin', 'Sinaia, Romania', 'Forest Lane 22',
'Quiet cabin surrounded by forests, ideal for relaxation.',
130, 4, 2, 1, 'Cabin', '/images/stays/cabin3.jpg', 4.7, 1, GETDATE()),

-- 🇬🇧 UK
('London Modern Flat', 'London, UK', 'Baker Street 221B',
'Stylish flat in central London with easy access to transport.',
260, 2, 1, 1, 'Apartment', '/images/stays/apartment5.jpg', 4.6, 1, GETDATE()),

-- 🇦🇪 DUBAI
('Dubai Marina Luxury Suite', 'Dubai, UAE', 'Marina Walk 5',
'Luxury suite with skyline and marina views.',
420, 2, 1, 1, 'Studio', '/images/stays/studio3.jpg', 4.9, 1, GETDATE()),

-- 🇹🇭 THAILAND
('Phuket Beach Villa', 'Phuket, Thailand', 'Beach Road 9',
'Tropical villa with private pool and direct beach access.',
390, 5, 3, 2, 'Villa', '/images/stays/villa3.jpg', 4.8, 1, GETDATE()),

-- 🇯🇵 JAPAN
('Tokyo Minimal Apartment', 'Tokyo, Japan', 'Shibuya 11',
'Compact and modern apartment in vibrant Shibuya.',
180, 2, 1, 1, 'Apartment', '/images/stays/apartment6.jpg', 4.5, 1, GETDATE()),

-- 🇺🇸 USA
('New York Skyline Studio', 'New York, USA', 'Manhattan 5th Ave',
'Studio with amazing skyline views in Manhattan.',
320, 2, 1, 1, 'Studio', '/images/stays/studio4.jpg', 4.7, 1, GETDATE()),

-- 🇨🇦 CANADA
('Toronto Downtown Condo', 'Toronto, Canada', 'King Street 99',
'Modern condo in downtown Toronto with city views.',
200, 3, 2, 1, 'Apartment', '/images/stays/apartment7.jpg', 4.6, 1, GETDATE()),

-- 🇮🇸 ICELAND
('Iceland Northern Lights Cabin', 'Reykjavik, Iceland', 'Aurora Road 4',
'Cabin perfect for viewing the Northern Lights.',
250, 4, 2, 1, 'Cabin', '/images/stays/cabin4.jpg', 4.9, 1, GETDATE());
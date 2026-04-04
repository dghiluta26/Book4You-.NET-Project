SELECT * FROM Users
SELECT * FROM Reviews
SELECT * FROM Bookings
SELECT * FROM Accommodations
SELECT * FROM AccommodationAmenities
SELECT * FROM Amenities


INSERT INTO Amenities (Name)
VALUES
('WiFi'),
('Pool'),
('Parking'),
('Air Conditioning'),
('Breakfast included'),
('Kitchen'),
('TV'),
('Pet friendly');


INSERT INTO AccommodationAmenities (AccommodationId, AmenityId)
VALUES
(3, 1), -- cazarea 1 are WiFi
(3, 2), -- cazarea 1 are Pool
(3, 4); -- cazarea 1 are Air Conditioning

INSERT INTO AccommodationImages (AccommodationId, ImageUrl, IsMain, DisplayOrder)
VALUES
(3, '/images/stays/villa1.jpg', 1, 1),
(3, '/images/stays/villa1_2.jpg', 0, 2),
(3, '/images/stays/villa1_3.jpg', 0, 3);

INSERT INTO AccommodationImages (AccommodationId, ImageUrl, IsMain, DisplayOrder)
VALUES
(7, '/images/stays/villa2.jpg', 1 , 1),
(7, '/images/stays/villa2_2.jpg', 0 , 2),
(7, '/images/stays/villa2_3.jpg', 0 , 3);

-- Migration: Populate Glaze Catalogue
-- Purpose: Insert initial glaze catalogue data for user reference
-- Date: 2025-01-06

-- Insert mid-fire glazes (Cone 5-6)
INSERT INTO public.glazes (user_id, name, manufacturer, type_id, color, cone_rating, food_safe, properties, notes, sync_status) VALUES
-- Mayco Stoneware Glazes
('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'SW-001 White Opal', 'Mayco', 2, 'White', 'Cone 5-6', true,
'{
  "firing": {
    "temperatureMin": 1180,
    "temperatureMax": 1240,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {20260106120000_populate_glaze_catalogue
    "transparency": "Opaque",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": []
  },
  "behavior": {
    "meltFluidity": "Medium",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Dipping", "Pouring"],
    "recommendedThickness": "2-3 coats",
    "applicationNotes": "Apply evenly for best results"
  },
  "clayCompatibility": {
    "bestSuited": ["Stoneware"],
    "interaction": "Excellent for stoneware bodies"
  },
  "usage": {
    "workType": "Functional",
    "durability": "High"
  }
}', 'Stable, classic white glaze for stoneware', 'SYNCED'),

('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'SW-002 Dark Flux', 'Mayco', 2, 'Dark Brown', 'Cone 5-6', false,
'{
  "firing": {
    "temperatureMin": 1180,
    "temperatureMax": 1240,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Medium"
  },
  "appearance": {
    "transparency": "Transparent",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": ["Reactive"]
  },
  "behavior": {
    "meltFluidity": "High",
    "thicknessTolerance": "Medium",
    "colorStability": "Medium",
    "repeatability": "Medium"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Dipping", "Pouring"],
    "recommendedThickness": "2 coats",
    "applicationNotes": "Reactive glaze - results vary with clay composition"
  },
  "clayCompatibility": {
    "bestSuited": ["Stoneware", "Porcelain"],
    "interaction": "Reactive with iron in clay"
  },
  "usage": {
    "workType": "Artistic",
    "durability": "Medium"
  }
}', 'Reactive dark flux glaze', 'SYNCED'),

('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'SW-402 Blue Surf', 'Mayco', 2, 'Blue', 'Cone 5-6', true,
'{
  "firing": {
    "temperatureMin": 1180,
    "temperatureMax": 1240,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {
    "transparency": "Opaque",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": ["Layering effects"]
  },
  "behavior": {
    "meltFluidity": "Medium",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Brushing", "Dipping"],
    "recommendedThickness": "2-3 coats",
    "applicationNotes": "Excellent for layering techniques"
  },
  "clayCompatibility": {
    "bestSuited": ["Stoneware"],
    "interaction": "Good layering properties"
  },
  "usage": {
    "workType": "Decorative",
    "durability": "High"
  }
}', 'Versatile blue glaze with layering capabilities', 'SYNCED'),

-- AMACO Potter's Choice Glazes
('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'PC-1 Clear Gloss', 'AMACO', 2, 'Clear', 'Cone 6', true,
'{
  "firing": {
    "temperatureMin": 1200,
    "temperatureMax": 1220,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {
    "transparency": "Transparent",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": []
  },
  "behavior": {
    "meltFluidity": "Medium",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Dipping", "Pouring", "Spraying"],
    "recommendedThickness": "2-3 coats",
    "applicationNotes": "Universal clear glaze"
  },
  "clayCompatibility": {
    "bestSuited": ["Stoneware", "Porcelain"],
    "interaction": "Compatible with most clay bodies"
  },
  "usage": {
    "workType": "Functional",
    "durability": "High"
  }
}', 'Professional clear gloss glaze', 'SYNCED'),

('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'PC-10 Turquoise', 'AMACO', 2, 'Turquoise', 'Cone 6', true,
'{
  "firing": {
    "temperatureMin": 1200,
    "temperatureMax": 1220,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {
    "transparency": "Opaque",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": []
  },
  "behavior": {
    "meltFluidity": "Medium",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Brushing", "Dipping"],
    "recommendedThickness": "2 coats",
    "applicationNotes": "Vibrant turquoise color"
  },
  "clayCompatibility": {
    "bestSuited": ["Stoneware", "Porcelain"],
    "interaction": "Stable color development"
  },
  "usage": {
    "workType": "Decorative",
    "durability": "High"
  }
}', 'Bright turquoise glaze', 'SYNCED'),

('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'PC-50 Matte White', 'AMACO', 2, 'White', 'Cone 6', true,
'{
  "firing": {
    "temperatureMin": 1200,
    "temperatureMax": 1220,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {
    "transparency": "Opaque",
    "finish": "Matte",
    "texture": ["Smooth"],
    "specialEffects": []
  },
  "behavior": {
    "meltFluidity": "Low",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Dipping", "Pouring"],
    "recommendedThickness": "2-3 coats",
    "applicationNotes": "Matte finish white glaze"
  },
  "clayCompatibility": {
    "bestSuited": ["Stoneware", "Porcelain"],
    "interaction": "Excellent opacity"
  },
  "usage": {
    "workType": "Functional",
    "durability": "High"
  }
}', 'Matte white glaze for stoneware', 'SYNCED');

-- Insert low-fire glazes (Cone 06-04)
INSERT INTO public.glazes (user_id, name, manufacturer, type_id, color, cone_rating, food_safe, properties, notes, sync_status) VALUES
-- Mayco Fundamentals / Stroke & Coat
('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'Clear Gloss', 'Mayco', 1, 'Clear', 'Cone 06-04', true,
'{
  "firing": {
    "temperatureMin": 1000,
    "temperatureMax": 1080,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {
    "transparency": "Transparent",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": []
  },
  "behavior": {
    "meltFluidity": "Medium",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Dipping", "Pouring", "Brushing"],
    "recommendedThickness": "2-3 coats",
    "applicationNotes": "Universal clear glaze for low-fire"
  },
  "clayCompatibility": {
    "bestSuited": ["Earthenware"],
    "interaction": "Compatible with most low-fire clays"
  },
  "usage": {
    "workType": "Functional",
    "durability": "Medium"
  }
}', 'Universal clear gloss for low-fire ceramics', 'SYNCED'),

('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'SC-16 Cotton Tail', 'Mayco', 1, 'White', 'Cone 06-04', true,
'{
  "firing": {
    "temperatureMin": 1000,
    "temperatureMax": 1080,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {
    "transparency": "Opaque",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": []
  },
  "behavior": {
    "meltFluidity": "Medium",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Brushing", "Dipping"],
    "recommendedThickness": "2 coats",
    "applicationNotes": "Soft white decorative glaze"
  },
  "clayCompatibility": {
    "bestSuited": ["Earthenware"],
    "interaction": "Good for decorative work"
  },
  "usage": {
    "workType": "Decorative",
    "durability": "Medium"
  }
}', 'Soft white glaze for decorative ceramics', 'SYNCED'),

('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'SC-73 Candy Apple Red', 'Mayco', 1, 'Red', 'Cone 06-04', true,
'{
  "firing": {
    "temperatureMin": 1000,
    "temperatureMax": 1080,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {
    "transparency": "Opaque",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": []
  },
  "behavior": {
    "meltFluidity": "Medium",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Brushing"],
    "recommendedThickness": "2-3 coats",
    "applicationNotes": "Bright red color - apply evenly"
  },
  "clayCompatibility": {
    "bestSuited": ["Earthenware"],
    "interaction": "Vibrant color development"
  },
  "usage": {
    "workType": "Decorative",
    "durability": "Medium"
  }
}', 'Bright candy apple red glaze', 'SYNCED');

-- Insert high-fire glazes (Cone 9-10)
INSERT INTO public.glazes (user_id, name, manufacturer, type_id, color, cone_rating, food_safe, properties, notes, sync_status) VALUES
('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'Celadon Clear', 'Generic', 3, 'Pale Green', 'Cone 9-10', true,
'{
  "firing": {
    "temperatureMin": 1280,
    "temperatureMax": 1300,
    "temperatureUnit": "C",
    "atmosphere": "Reduction",
    "curveSensitivity": "Medium"
  },
  "appearance": {
    "transparency": "Semi-transparent",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": ["Crystalline"]
  },
  "behavior": {
    "meltFluidity": "Low",
    "thicknessTolerance": "Medium",
    "colorStability": "Medium",
    "repeatability": "Medium"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Dipping", "Pouring"],
    "recommendedThickness": "2 coats",
    "applicationNotes": "Best fired in reduction atmosphere"
  },
  "clayCompatibility": {
    "bestSuited": ["Porcelain", "Stoneware"],
    "interaction": "Traditional celadon requires proper clay composition"
  },
  "usage": {
    "workType": "Artistic",
    "durability": "High"
  }
}', 'Traditional Chinese-style celadon glaze', 'SYNCED'),

('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'Shino', 'Generic', 3, 'Cream', 'Cone 10', true,
'{
  "firing": {
    "temperatureMin": 1280,
    "temperatureMax": 1320,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "High"
  },
  "appearance": {
    "transparency": "Semi-transparent",
    "finish": "Satin",
    "texture": ["Ash-like"],
    "specialEffects": ["Running", "Cracking"]
  },
  "behavior": {
    "meltFluidity": "High",
    "thicknessTolerance": "Low",
    "colorStability": "Low",
    "repeatability": "Low"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Brushing", "Spraying"],
    "recommendedThickness": "1-2 coats",
    "applicationNotes": "Apply thinly - tends to run"
  },
  "clayCompatibility": {
    "bestSuited": ["Stoneware"],
    "interaction": "Creates ash-like surface texture"
  },
  "usage": {
    "workType": "Artistic",
    "durability": "High"
  }
}', 'Traditional Japanese shino glaze with running effects', 'SYNCED'),

('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'Tenmoku', 'Generic', 3, 'Black', 'Cone 10', true,
'{
  "firing": {
    "temperatureMin": 1280,
    "temperatureMax": 1300,
    "temperatureUnit": "C",
    "atmosphere": "Reduction",
    "curveSensitivity": "Medium"
  },
  "appearance": {
    "transparency": "Opaque",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": ["Oil-spot"]
  },
  "behavior": {
    "meltFluidity": "Medium",
    "thicknessTolerance": "Medium",
    "colorStability": "Medium",
    "repeatability": "Medium"
  },
  "application": {
    "form": "Liquid",
    "methods": ["Dipping", "Pouring"],
    "recommendedThickness": "2 coats",
    "applicationNotes": "Fired in reduction for best results"
  },
  "clayCompatibility": {
    "bestSuited": ["Stoneware"],
    "interaction": "Creates oil-spot effects in reduction"
  },
  "usage": {
    "workType": "Artistic",
    "durability": "High"
  }
}', 'Traditional tenmoku glaze with oil-spot effects', 'SYNCED');

-- Insert powdered glazes
INSERT INTO public.glazes (user_id, name, manufacturer, type_id, color, cone_rating, food_safe, properties, notes, sync_status) VALUES
('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'Transparent 402200', 'Generic', 2, 'Clear', 'Cone 5-6', true,
'{
  "firing": {
    "temperatureMin": 1180,
    "temperatureMax": 1250,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {
    "transparency": "Transparent",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": []
  },
  "behavior": {
    "meltFluidity": "Medium",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Powder",
    "methods": ["Dipping", "Pouring"],
    "recommendedThickness": "2-3 coats",
    "applicationNotes": "Mix with water to desired consistency"
  },
  "clayCompatibility": {
    "bestSuited": ["Stoneware", "Porcelain"],
    "interaction": "Excellent transparency"
  },
  "usage": {
    "workType": "Functional",
    "durability": "High"
  }
}', 'Professional clear glaze powder', 'SYNCED'),

('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'Transparent 402103', 'Generic', 1, 'Clear', 'Cone 06-04', true,
'{
  "firing": {
    "temperatureMin": 1000,
    "temperatureMax": 1150,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {
    "transparency": "Transparent",
    "finish": "Glossy",
    "texture": ["Smooth"],
    "specialEffects": []
  },
  "behavior": {
    "meltFluidity": "Medium",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Powder",
    "methods": ["Dipping", "Pouring"],
    "recommendedThickness": "2-3 coats",
    "applicationNotes": "Suitable for earthenware - mix with water"
  },
  "clayCompatibility": {
    "bestSuited": ["Earthenware"],
    "interaction": "Compatible with low-fire clays"
  },
  "usage": {
    "workType": "Functional",
    "durability": "Medium"
  }
}', 'Clear glaze powder for earthenware', 'SYNCED'),

('5fecee93-19a3-4648-b201-a8b9e8b8b37b', 'White Matt', 'Generic', 2, 'White', 'Cone 5-6', true,
'{
  "firing": {
    "temperatureMin": 1180,
    "temperatureMax": 1220,
    "temperatureUnit": "C",
    "atmosphere": "Oxidation",
    "curveSensitivity": "Low"
  },
  "appearance": {
    "transparency": "Opaque",
    "finish": "Matte",
    "texture": ["Smooth"],
    "specialEffects": []
  },
  "behavior": {
    "meltFluidity": "Low",
    "thicknessTolerance": "High",
    "colorStability": "High",
    "repeatability": "High"
  },
  "application": {
    "form": "Powder",
    "methods": ["Dipping", "Pouring"],
    "recommendedThickness": "2-3 coats",
    "applicationNotes": "Matte finish - mix with water"
  },
  "clayCompatibility": {
    "bestSuited": ["Stoneware", "Porcelain"],
    "interaction": "Excellent opacity and matte finish"
  },
  "usage": {
    "workType": "Functional",
    "durability": "High"
  }
}', 'Opaque matte white glaze powder', 'SYNCED');

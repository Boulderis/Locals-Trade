﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestAPI.Cryptography;
using RestAPI.Models;

namespace RestAPI.Infrastructure
{
    public class SeedData
    {

        public static void EnsurePopulated(IApplicationBuilder app, HashCalculator hashCalculator, Imager imager)
        {
            ServiceDbContext context = app.ApplicationServices.CreateScope().ServiceProvider
                .GetRequiredService<ServiceDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            if (!context.Users.Any())
            {
                context.Users.AddRange(CreateTestMaterial(hashCalculator, imager));
                context.SaveChanges();
            }
        }

        private static User CreateUser(string name, string surname, int year, int month, int day, string email, string passhash)
        {
            return new User { Name = name, Surname = surname, BirthDate = new DateTime(year, month, day), Email = email, Passhash = passhash };
        }

        private static Business CreateBusiness(string description, string longitude, string latitude, string phone, string header, byte[] image)
        {
            return new Business
            {
                Description = description,
                Longitude = longitude,
                Latitude = latitude,
                PhoneNumber = phone,
                Header = header,
                PictureData = image
            };
        }

        private static Product CreateProduct(string name, decimal unitPrice, string unit, string comment, byte[] image)
        {
            return new Product { Name = name, PricePerUnit = unitPrice, Unit = unit, Comment = comment, PictureData = image };
        }

        private static TimeSheet CreateWorkday(int fh, int fm, int th, int tm, int day)
        {
            return new TimeSheet
            {
                From = new DateTime(1999, 12, 06, fh, fm, 00),
                To = new DateTime(1999, 12, 06, th, tm, 00),
                Weekday = day
            };
        }

        private static User[] CreateTestMaterial(HashCalculator hashCalculator, Imager imager)
        {
            User u1 = CreateUser("Evaldas", "Visockas", 1999, 12, 06, "vievaldas@gmail.com", hashCalculator.PassHash("lydeka"));
            User u2 = CreateUser("Šarūnas", "Teisutis", 1996, 04, 02, "testinis1422@gmail.com", hashCalculator.PassHash("tigras"));
            User u3 = CreateUser("Kazys", "Bruolė", 1950, 01, 02, "senovinis5478@gmail.com", hashCalculator.PassHash("katinas"));
            User u4 = CreateUser("Birutė", "Išdykėlė", 2001, 01, 15, "atsitiktinis3456@gmail.com", hashCalculator.PassHash("miau"));
            User u5 = CreateUser("Lapinas", "Baronas", 2005, 10, 15, "lapinas3456@gmail.com", hashCalculator.PassHash("uosis"));
            User u6 = CreateUser("Konradas", "Rado", 2018, 06, 05, "konradas5423@gmail.com", hashCalculator.PassHash("cimbaliukas"));
            User u7 = CreateUser("Vaiva", "Vaivorykštė", 1961, 07, 02, "vaiva7423@gmail.com", hashCalculator.PassHash("paprika"));
            User u8 = CreateUser("Vaidas", "Grinius", 2004, 03, 06, "vaidas12354672@gmail.com", hashCalculator.PassHash("lapinas"));
            User u9 = CreateUser("Laputė", "Laisvuolė", 1930, 02, 15, "lapute422068@gmail.com", hashCalculator.PassHash("kaziukas"));
            User u10 = CreateUser("Kazys", "Keistuolis", 1994, 04, 12, "keistuolis4122315@gmail.com", hashCalculator.PassHash("greitkelis"));
            User u11 = CreateUser("Rasa", "Išplaukusi", 2000, 01, 03, "isplaukusi423456@gmail.com", hashCalculator.PassHash("popkornas"));

            Business b1 = CreateBusiness("Traktorių parduotuvė", "25.274633", "54.699603", "+37064575620", "10% nuolaida žieminėms padangoms", null);
            Business b2 = CreateBusiness("Daužti automobiliai už prieinamą kainą", "25.268811", "54.693882", "+37064215675", "Įsigykite automobilius be rėmo, greit neliks", null);
            Business b3 = CreateBusiness("Įvairūs", "25.269173", "54.688853", "+37064215201", "Saldainių parduotuvė", null);
            Business b4 = CreateBusiness("Smagūs niekučiai", "25.281194", "54.681308", "+37064512012", "Išpardavimas išdaigininkams", null);
            Business b5 = CreateBusiness("Nuo 7:00 iki 8:00 ryto remontuoju už dyką", "25.214340", "54.379052", "+37064512456", "Automobilių remontas", null);
            Business b6 = CreateBusiness("Rašau referatus, rašinius ir t.t.", "25.391008", "54.290225", "+37064752660", "Nenorit daryt mokyklinio projekto? Susisiekit, padarysiu už jus!", null);
            Business b7 = CreateBusiness("Galiu daryti portretus, natiurmortą, peisažą ir t.t.", "25.359637", "54.306365", "+37064233450", "Piešiniai už (beveik) dyką!", null);
            Business b8 = CreateBusiness("Paprastai labai kvepia", "25.304485", "54.630629", "+37064512478", "Yra ir smirdančių", null);
            Business b9 = CreateBusiness("Paaukokite senelių namuose gyvenantiems senjorams", "24.489809", "55.565052", "+37062199630", "Padarykite gerą darbą vieno mygtuko paspaudimu!", null);

            u1.Businesses.Add(b1);
            u1.Businesses.Add(b2);
            u2.Businesses.Add(b3);
            u2.Businesses.Add(b4);
            u3.Businesses.Add(b5);
            u3.Businesses.Add(b6);
            u4.Businesses.Add(b7);
            u5.Businesses.Add(b8);
            u6.Businesses.Add(b9);

            Product p1 = CreateProduct("Traktorius", 10000, "vienetas", "Liko tik trys.", null);
            Product p2 = CreateProduct("Daužtas BMW", 20000, "vienetas", "Jei netinka, galim dar padaužti prieš parduodant.", null);
            Product p3 = CreateProduct("Nissan", 10000, "vienetas", "Sena gera mašina.", null);
            Product p4 = CreateProduct("Saldainiai arbūzai", 3, "pakelis", "Tik močiutės pyragai skanesni.", null);
            Product p5 = CreateProduct("Sproginėjantys čiulpinukai", 2, "čiulpinukas", "Suvalgius keisti garsai pilve girdisi.", null);
            Product p6 = CreateProduct("Remonto paslauga", 350, "sutaisytas automobilis", "Paprastai sutaisai per valandą dvi.", null);
            Product p7 = CreateProduct("Kontrolinių atsakymai", 20, "lapas", "Pametus garantija neteikiama.", null);
            Product p8 = CreateProduct("Peisažas", 200, "piešinys",
                "Perkant du ar daugiau, taikoma 20% nuolaida bendrai sumai", null);
            Product p9 = CreateProduct("Natiurmortas", 500, "piešinys",
                "Perkant du ar daugiau, mokate 20% brangiau bendros sumos", null);
            Product p10 = CreateProduct("Kvepiančios kojinės", 25.73M, "viena kojinė", "Išnešiojo srities specialistas", null);
            Product p11 = CreateProduct("Pinigų aukojimas", 5, "kartas", "Jei norite, galite paaukoti ir daugiau.", null);

            b1.Products.Add(p1);
            b1.Products.Add(p2);
            b2.Products.Add(p3);
            b3.Products.Add(p4);
            b4.Products.Add(p5);
            b5.Products.Add(p6);
            b6.Products.Add(p7);
            b7.Products.Add(p8);
            b7.Products.Add(p9);
            b8.Products.Add(p10);
            b9.Products.Add(p11);

            TimeSheet t1 = CreateWorkday(10, 0, 11, 0, 1);
            TimeSheet t2 = CreateWorkday(10, 0, 16, 0, 3);
            TimeSheet t3 = CreateWorkday(10, 0, 12, 0, 2);
            TimeSheet t4 = CreateWorkday(8, 0, 12, 0, 1);
            TimeSheet t5 = CreateWorkday(8, 0, 11, 0, 2);
            TimeSheet t6 = CreateWorkday(8, 0, 11, 0, 6);
            TimeSheet t7 = CreateWorkday(7, 0, 9, 0, 4);
            TimeSheet t8 = CreateWorkday(7, 0, 16, 30, 1);
            TimeSheet t9 = CreateWorkday(7, 0, 16, 30, 2);
            TimeSheet t10 = CreateWorkday(7, 0, 16, 30, 3);
            TimeSheet t11 = CreateWorkday(7, 0, 16, 30, 4);
            TimeSheet t12 = CreateWorkday(7, 0, 16, 30, 5);
            TimeSheet t13 = CreateWorkday(1, 0, 23, 30, 7);

            b1.Workdays.Add(t1);
            b1.Workdays.Add(t2);
            b2.Workdays.Add(t3);
            b2.Workdays.Add(t4);
            b3.Workdays.Add(t5);
            b4.Workdays.Add(t6);
            b5.Workdays.Add(t7);
            b5.Workdays.Add(t8);
            b8.Workdays.Add(t9);
            b8.Workdays.Add(t10);
            b8.Workdays.Add(t11);
            b8.Workdays.Add(t12);
            b8.Workdays.Add(t13);

            Product p10_0 = CreateProduct("Blackberry", 15, "1 Kilo", "Fresh forest blackberries", null);
            Product p10_1 = CreateProduct("Fragaria", 8, "1 Kilo", "Sweet, medium sized fragarias", null);
            Business b10 = CreateBusiness("Selling some kind of wild berries", "25.235131", "54.758748", "+37062199630", "Wild berries", imager.ByteMaker("10.jpg"));
            TimeSheet t10_0 = CreateWorkday(8, 0, 18, 0, 3);

            u6.Businesses.Add(b10);
            b10.Products.Add(p10_0);
            b10.Products.Add(p10_1);
            b10.Workdays.Add(t10_0);

            Product p11_0 = CreateProduct("Strawberry", 4, "1 Kilo", "Frozen strawberries", null);
            Product p11_1 = CreateProduct("Raspberry", 7, "1 Kilo", "Juicy raspberries", null);
            Business b11 = CreateBusiness("Selling berries from my garden", "25.266962", "54.792729", "+37062199630", "Berries", imager.ByteMaker("11.jpg"));
            TimeSheet t11_0 = CreateWorkday(10, 30, 17, 0, 2);
            TimeSheet t11_1 = CreateWorkday(8, 45, 18, 30, 4);


            u6.Businesses.Add(b11);
            b11.Products.Add(p11_0);
            b11.Products.Add(p11_1);
            b11.Workdays.Add(t11_0);
            b11.Workdays.Add(t11_1);

            Product p12_0 = CreateProduct("Welding", 20, "1 hour", "Can weld metal things", null);
            Product p12_1 = CreateProduct("Electrical work", 17, "1 hour", "All that connected to electrical work in home", null);
            Business b12 = CreateBusiness("I can weld, electrical work", "25.298652", "54.783504", "+37062199111", "Services", imager.ByteMaker("12.jpg"));
            TimeSheet t12_0 = CreateWorkday(8, 30, 22, 0, 1);

            u10.Businesses.Add(b12);
            b12.Products.Add(p12_0);
            b12.Products.Add(p12_1);
            b12.Workdays.Add(t12_0);

            Product p13_0 = CreateProduct("Plowing", 30, "1 ar", "Plow your ground", null);
            Business b13 = CreateBusiness("I can plow", "25.298652", "54.783504", "+37062199111", "Farming work", imager.ByteMaker("13.jpg"));
            TimeSheet t13_0 = CreateWorkday(8, 30, 22, 0, 1);

            u10.Businesses.Add(b13);
            b13.Products.Add(p13_0);
            b13.Workdays.Add(t13_0);

            Product p14_0 = CreateProduct("Sewing", 18, "1 cm2", "Order unique clothes", null);
            Business b14 = CreateBusiness("You can order clothes to sew", "25.277352", "54.737149", "+37062199222", "Sewing", imager.ByteMaker("14.jpg"));
            TimeSheet t14_0 = CreateWorkday(8, 30, 22, 0, 1);

            u9.Businesses.Add(b14);
            b14.Products.Add(p14_0);
            b14.Workdays.Add(t14_0);

            Product p15_0 = CreateProduct("Knitting", 28, "1 cm2", "Order unique clothes", null);
            Business b15 = CreateBusiness("You can order knited clothes", "25.277352", "54.737149", "+37062199222", "Knitting", imager.ByteMaker("15.jpg"));
            TimeSheet t15_0 = CreateWorkday(8, 30, 22, 0, 1);

            u9.Businesses.Add(b15);
            b15.Products.Add(p15_0);
            b15.Workdays.Add(t15_0);

            Product p16_0 = CreateProduct("Clothes", 1, "1 Clothe", "I can give some discount if you buy more", null);
            Business b16 = CreateBusiness("Selling used clothes for symbolic price", "25.226498", "54.708795", "+37062199222", "Clothes", imager.ByteMaker("16.jpg"));
            TimeSheet t16_0 = CreateWorkday(8, 30, 22, 0, 1);

            u8.Businesses.Add(b16);
            b16.Products.Add(p16_0);
            b16.Workdays.Add(t16_0);

            Product p17_0 = CreateProduct("Milk", 1, "1 Liter", "Fresh cow milk", null);
            Product p17_1 = CreateProduct("Curd", 1, "200 Grams", "Loose curd", null);
            Business b17 = CreateBusiness("Selling home made dairy products", "25.506973", "54.676054", "+37062199333", "Dairy", imager.ByteMaker("17.jpg"));
            TimeSheet t17_0 = CreateWorkday(8, 30, 12, 0, 1);
            TimeSheet t17_1 = CreateWorkday(8, 30, 12, 0, 2);
            TimeSheet t17_2 = CreateWorkday(8, 30, 12, 0, 3);
            TimeSheet t17_3 = CreateWorkday(8, 30, 12, 0, 4);
            TimeSheet t17_4 = CreateWorkday(8, 30, 12, 0, 5);
            TimeSheet t17_5 = CreateWorkday(8, 30, 12, 0, 6);
            TimeSheet t17_6 = CreateWorkday(8, 30, 12, 0, 7);


            u7.Businesses.Add(b17);
            b17.Products.Add(p17_0);
            b17.Products.Add(p17_1);
            b17.Workdays.Add(t17_0);
            b17.Workdays.Add(t17_1);
            b17.Workdays.Add(t17_2);
            b17.Workdays.Add(t17_3);
            b17.Workdays.Add(t17_4);
            b17.Workdays.Add(t17_5);
            b17.Workdays.Add(t17_6);

            Product p18_0 = CreateProduct("Chicken eggs", 3, "10 eggs", "Chickens were feeded without compound feedingstuffs", null);
            Business b18 = CreateBusiness("Selling chicken eggs", "25.326122", "54.638121", "+37062199222", "Eggs", imager.ByteMaker("18.jpg"));
            TimeSheet t18_0 = CreateWorkday(8, 30, 22, 0, 1);

            u8.Businesses.Add(b18);
            b18.Products.Add(p18_0);
            b18.Workdays.Add(t18_0);

            Product p19_0 = CreateProduct("Milk", 4, "1 Liter", "Fresh goat milk", null);
            Product p19_1 = CreateProduct("Cheese", 5, "200 Grams", "Goat cheese. Good with bread", null);
            Business b19 = CreateBusiness("Selling home made goat dairy products", "25.506973", "54.676054", "+37062199333", "Dairy", imager.ByteMaker("19.jpg"));
            TimeSheet t19_0 = CreateWorkday(9, 30, 12, 0, 1);
            TimeSheet t19_1 = CreateWorkday(9, 30, 12, 0, 2);
            TimeSheet t19_2 = CreateWorkday(9, 30, 12, 0, 3);
            TimeSheet t19_3 = CreateWorkday(9, 30, 12, 0, 4);
            TimeSheet t19_4 = CreateWorkday(9, 30, 12, 0, 5);
            TimeSheet t19_5 = CreateWorkday(9, 30, 12, 0, 6);
            TimeSheet t19_6 = CreateWorkday(9, 30, 12, 0, 7);


            u8.Businesses.Add(b19);
            b19.Products.Add(p19_0);
            b19.Products.Add(p19_1);
            b19.Workdays.Add(t19_0);
            b19.Workdays.Add(t19_1);
            b19.Workdays.Add(t19_2);
            b19.Workdays.Add(t19_3);
            b19.Workdays.Add(t19_4);
            b19.Workdays.Add(t19_5);
            b19.Workdays.Add(t19_6);

            Product p20_0 = CreateProduct("Bread", 3, "500 Grams", "Rectangle shaped bread", null);
            Business b20 = CreateBusiness("Selling home made bread from my own wheat", "25.301730", "54.636929", "+37062199333", "Bread", imager.ByteMaker("12.jpg"));
            TimeSheet t20_0 = CreateWorkday(8, 30, 22, 0, 1);

            u8.Businesses.Add(b20);
            b20.Products.Add(p20_0);
            b20.Workdays.Add(t20_0);

            Product p21_0 = CreateProduct("Goat", 50, "1 animal", "Gives 3liters of milk", null);
            Business b21 = CreateBusiness("Selling 1 year old goat", "25.301730", "54.636929", "+37062199333", "Goat", imager.ByteMaker("21.jfif"));
            TimeSheet t21_0 = CreateWorkday(8, 30, 22, 0, 1);

            u8.Businesses.Add(b21);
            b21.Products.Add(p21_0);
            b21.Workdays.Add(t21_0);

            Business b22 = CreateBusiness("Giving cat for symbolic price", "25.262359", "54.639473", "+37062199331", "Cat", null);
            Product p22 = CreateProduct("Cat", 1, "1 animal", "Small cat", null);
            TimeSheet t22 = CreateWorkday(8, 30, 22, 0, 1);

            u1.Businesses.Add(b22);
            b22.Products.Add(p22);
            b22.Workdays.Add(t22);

            Business b23 = CreateBusiness("Selling 1 year German Shepherd", "25.205261", "54.665214", "+37062199332", "Dog", null);
            Product p23 = CreateProduct("German Shepherd", 150, "1 animal", "Needs new home", null);
            TimeSheet t23 = CreateWorkday(8, 30, 22, 0, 1);

            u2.Businesses.Add(b23);
            b23.Products.Add(p23);
            b23.Workdays.Add(t23);

            Business b24 = CreateBusiness("Selling 1.5 year old green lizard", "25.249167", "54.708795", "+37062199330", "Lizard", null);
            Product p24_0 = CreateProduct("Lizard", 150, "1 animal", "It is 1.23 meters long", null);
            TimeSheet t24_0 = CreateWorkday(8, 30, 22, 0, 1);

            u3.Businesses.Add(b24);
            b24.Products.Add(p24_0);
            b24.Workdays.Add(t24_0);

            Business b25 = CreateBusiness("Selling newborn white guinea pigs", "25.301730", "54.636929", "+37062199334", "Guinea pig", null);
            Product p25_0 = CreateProduct("Guinea pig", 50, "1 animal", "Newborn Guinea pigs", null);

            u4.Businesses.Add(b25);
            b25.Products.Add(p25_0);
            b25.Workdays.Add(t21_0);

            Business b26 = CreateBusiness("Fixing your phones", "25.309975", "54.706812", "+37062199355", "Repair job", null);
            Product p26_0 = CreateProduct("Broken glass", 100, "1 screen", "Price depends on selected phone", null);

            u4.Businesses.Add(b26);
            b26.Products.Add(p26_0);
            b26.Workdays.Add(t21_0);


            Business b27 = CreateBusiness("Fixing your watches", "25.309975", "54.706812", "+37062199355", "Repair job", null);
            Product p27_0 = CreateProduct("Fix watch", 100, "unit", "Price depends by work", null);

            u4.Businesses.Add(b27);
            b27.Products.Add(p27_0);
            b27.Workdays.Add(t21_0);


            Business b28 = CreateBusiness("Fixing your laptops", "25.300356", "54.678635", "+37062199356", "Repair job", null);
            Product p28_0 = CreateProduct("Laptop", 200, "unit", "Price depends on work dificulty", null);

            u5.Businesses.Add(b28);
            b28.Products.Add(p28_0);
            b28.Workdays.Add(t21_0);


            Business b29 = CreateBusiness("Fast applying screen protecting glass", "25.257756", "54.679627", "+37062199356", "Repair job", null);
            Product p29_0 = CreateProduct("Apply job", 5, "1 glass", "Applying protective glass on your phone in 2 minutes. Dont forget to bring your glass.", null);

            u5.Businesses.Add(b29);
            b29.Products.Add(p29_0);
            b29.Workdays.Add(t21_0);


            Business b30 = CreateBusiness("Fast changing car tires", "25.253633", "54.711374", "+37062199374", "Repair job", null);
            Product p30_0 = CreateProduct("Tire installation", 40, "1 tire", "Price depends on selected tire", null);

            u5.Businesses.Add(b30);
            b30.Products.Add(p30_0);
            b30.Workdays.Add(t21_0);


            Business b31 = CreateBusiness("Fast changing engine oil", "25.228210", "54.702249", "+37062199355", "Repair job", null);
            Product p31_0 = CreateProduct("Oil change", 30, "one service", "Price includes only work", null);

            u6.Businesses.Add(b31);
            b31.Products.Add(p31_0);
            b31.Workdays.Add(t21_0);


            Business b32 = CreateBusiness("Fixing your phones", "25.309975", "54.706812", "+37062199358", "Eggs", null);
            Product p32_0 = CreateProduct("Quail eggs", 5, "60 eggs", "Small ones", null);

            u7.Businesses.Add(b32);
            b32.Products.Add(p32_0);
            b32.Workdays.Add(t21_0);


            Business b33 = CreateBusiness("Fresh bread kvass", "25.258030", "54.698995", "+37062199359", "Drink", null);
            Product p33_0 = CreateProduct("Kvass", 4, "1 liter", "Refreshing taste", null);

            u8.Businesses.Add(b33);
            b33.Products.Add(p33_0);
            b33.Workdays.Add(t21_0);


            Business b34 = CreateBusiness("Home made vine from apples", "25.180800", "54.725810", "+37062199388", "Drink", null);
            Product p34_0 = CreateProduct("Apple vine", 30, "1 bottle", "It is hold for 10 years", null);

            u9.Businesses.Add(b34);
            b34.Products.Add(p34_0);
            b34.Workdays.Add(t21_0);


            Business b35 = CreateBusiness("Amazing apple juice", "25.180800", "54.725810", "+37062199388", "Drink", null);
            Product p35_0 = CreateProduct("Bag of apple juice", 8, "5 liters", "Can hold 1 year if not opened", null);

            u9.Businesses.Add(b35);
            b35.Products.Add(p35_0);
            b35.Workdays.Add(t21_0);


            Business b36 = CreateBusiness("This season walnuts", "25.088591", "54.778514", "+37062199301", "Nuts", null);
            Product p36_0 = CreateProduct("Walnuts", 10, "1 kilo", "Nuts from lithuanian garden", null);

            u11.Businesses.Add(b36);
            b36.Products.Add(p36_0);
            b36.Workdays.Add(t21_0);


            Business b37 = CreateBusiness("Teaching math to pass exams", "25.249243", "54.692726", "+37062199222", "Tutor", null);
            Product p37_0 = CreateProduct("Lesson", 20, "1 hour", "For highschool graders", null);

            u1.Businesses.Add(b37);
            b37.Products.Add(p37_0);
            b37.Workdays.Add(t21_0);


            Business b38 = CreateBusiness("Profesional help for good results on exam", "25.262642", "54.680421", "+37062199374", "Tutor", null);
            Product p38_0 = CreateProduct("Lesson", 30, "1 hour", "For highschool graders", null);

            u2.Businesses.Add(b38);
            b38.Products.Add(p38_0);
            b38.Workdays.Add(t21_0);


            Business b39 = CreateBusiness("Try to pass exams and help to understand topics better", "25.276590", "54.750784", "+37062199365", "Tutor", null);
            Product p39_0 = CreateProduct("Lesson", 30, "1 hour", "For highschool graders", null);

            u3.Businesses.Add(b39);
            b39.Products.Add(p39_0);
            b39.Workdays.Add(t21_0);

            Business b40 = CreateBusiness("Help to prepare for exams", "25.241432", "54.807413", "+37062199332", "Tutor", null);
            Product p40_0 = CreateProduct("Lesson", 25, "1 hour", "For highschool graders", null);

            u4.Businesses.Add(b40);
            b40.Products.Add(p40_0);
            b40.Workdays.Add(t21_0);

            Product p41_0 = CreateProduct("Strawberry", 4, "1 Kilo", "My friend strawberries", null);
            Business b41 = CreateBusiness("Selling berries from my friends garden", "25.274860", "54.790408", "+37062199630", "Berries", imager.ByteMaker("11.jpg"));
            TimeSheet t41_0 = CreateWorkday(10, 30, 17, 0, 2);
            TimeSheet t41_1 = CreateWorkday(8, 45, 18, 30, 4);


            u1.Businesses.Add(b41);
            b41.Products.Add(p41_0);
            b41.Workdays.Add(t41_0);
            b41.Workdays.Add(t41_1);

            Product p42_0 = CreateProduct("Strawberry", 4, "1 Kilo", "Grandmas strawberries", null);
            Business b42 = CreateBusiness("Selling berries from my grandmas garden", "25.335676", "54.776059", "+37062199630", "Berries", imager.ByteMaker("11.jpg"));
            TimeSheet t42_0 = CreateWorkday(10, 30, 17, 0, 2);
            TimeSheet t42_1 = CreateWorkday(8, 45, 18, 30, 4);


            u2.Businesses.Add(b42);
            b42.Products.Add(p42_0);
            b42.Workdays.Add(t42_0);
            b42.Workdays.Add(t42_1);

            Product p43_0 = CreateProduct("Strawberry", 4, "1 Kilo", "Fresh strawberries", null);
            Business b43 = CreateBusiness("Selling berries", "25.331598", "54.728784", "+37062199630", "Berries", imager.ByteMaker("11.jpg"));
            TimeSheet t43_0 = CreateWorkday(10, 30, 17, 0, 2);
            TimeSheet t43_1 = CreateWorkday(8, 45, 18, 30, 4);


            u3.Businesses.Add(b43);
            b43.Products.Add(p43_0);
            b43.Workdays.Add(t43_0);
            b43.Workdays.Add(t43_1);

            Product p44_0 = CreateProduct("Strawberry", 4, "1 Kilo", "Regular strawberries", null);
            Business b44 = CreateBusiness("Selling berries", "25.153845", "54.713634", "+37062199630", "Berries", imager.ByteMaker("11.jpg"));
            TimeSheet t44_0 = CreateWorkday(10, 30, 17, 0, 2);
            TimeSheet t44_1 = CreateWorkday(8, 45, 18, 30, 4);


            u4.Businesses.Add(b44);
            b44.Products.Add(p44_0);
            b44.Workdays.Add(t44_0);
            b44.Workdays.Add(t44_1);

            Product p45_0 = CreateProduct("Švarkas", 11, "vienetas", "Raudonas švarkas", imager.ByteMaker("45.jpg"));
            Business b45 = CreateBusiness("Parduodami raudoni švarkai", "25.279652", "54.687157", "+37062169600", "Kokybiški švarkai", imager.ByteMaker("45.jpg"));
            TimeSheet t45_0 = CreateWorkday(10, 30, 17, 0, 2);
            TimeSheet t45_1 = CreateWorkday(8, 45, 18, 30, 4);


            u11.Businesses.Add(b45);
            b45.Products.Add(p45_0);
            b45.Workdays.Add(t45_0);
            b45.Workdays.Add(t45_1);

            Product p46_0 = CreateProduct("Švarkas", 9, "vienetas", "Pilkas švarkas", imager.ByteMaker("47.jpg"));
            Business b46 = CreateBusiness("Parduodami pilki švarkai", "25.252259", "54.690940", "+37062899030", "Iš Kinijos", imager.ByteMaker("47.jpg"));


            u10.Businesses.Add(b46);
            b46.Products.Add(p46_0);
            b46.Workdays.Add(t45_0);
            b46.Workdays.Add(t45_1);

            Product p47_0 = CreateProduct("Švarkas", 20, "vienetas", "Mėlynas švarkas", imager.ByteMaker("46.jpg"));
            Business b47 = CreateBusiness("Parduodami mėlyni švarkai", "25.350161", "54.741906", "+37062799630", "Pagaminti Lietuvoje", imager.ByteMaker("46.jpg"));


            u9.Businesses.Add(b47);
            b47.Products.Add(p47_0);
            b47.Workdays.Add(t45_0);
            b47.Workdays.Add(t45_1);

            Product p48_0 = CreateProduct("Švarkas", 12, "vienetas", "Įvairių spalvų švarkai", imager.ByteMaker("48.jpg"));
            Business b48 = CreateBusiness("Parduodami įvairių spalvų švarkai", "25.375241", "54.732590", "+37062699630", "Geros kokybės", imager.ByteMaker("48.jpg"));


            u7.Businesses.Add(b48);
            b48.Products.Add(p48_0);
            b48.Workdays.Add(t45_0);
            b48.Workdays.Add(t45_1);

            Product p49_0 = CreateProduct("Švarkas", 5, "vienetas", "Neblogos kokybės", imager.ByteMaker("49.jpg"));
            Business b49 = CreateBusiness("Parduodami devėti švarkai", "25.309607", "54.703836", "+37062599630", "Gana geros kokybės", imager.ByteMaker("49.jpg"));


            u8.Businesses.Add(b49);
            b49.Products.Add(p49_0);
            b49.Workdays.Add(t45_0);
            b49.Workdays.Add(t45_1);
            //---------------
            Product p50_0 = CreateProduct("Padangos", 80, "4x komplektas", "Naujos žieminės Continental padangos", imager.ByteMaker("50.jpg"));
            Business b50 = CreateBusiness("Parduodamos žieminės padangos", "25.276903", "54.668113", "+37061299630", "Puikus sukibimas", imager.ByteMaker("50.jpg"));

            u7.Businesses.Add(b50);
            b50.Products.Add(p50_0);
            b50.Workdays.Add(t45_0);
            b50.Workdays.Add(t45_1);

            Product p51_0 = CreateProduct("Padangos", 60, "4x komplektas", "Naujos žieminės ir vasarinės Viking padangos", imager.ByteMaker("51.jpg"));
            Business b51 = CreateBusiness("Parduodamos žieminės ir vasarinės padangos", "25.211628", "54.669304", "+37062009630", "Ilgaamžės ir patikimos", imager.ByteMaker("51.jpg"));


            u6.Businesses.Add(b51);
            b51.Products.Add(p51_0);
            b51.Workdays.Add(t45_0);
            b51.Workdays.Add(t45_1);

            Product p52_0 = CreateProduct("Padangos", 65, "4x komplektas", "Michelin universalios padangos", imager.ByteMaker("52.jpg"));
            Business b52 = CreateBusiness("Parduodamos naujos Michelin universalios padangos ", "25.170265", "54.788730", "+37062119630", "Geros ir žiema ir vasara", imager.ByteMaker("52.jpg"));


            u5.Businesses.Add(b52);
            b52.Products.Add(p52_0);
            b52.Workdays.Add(t45_0);
            b52.Workdays.Add(t45_1);

            Product p53_0 = CreateProduct("Padangos", 20, "2x komplektas", "Naudotos padangos", imager.ByteMaker("53.jpg"));
            Business b53 = CreateBusiness("Parduodamos naudotos padangos", "25.247907", "54.805355", "+37062229630", "Dar patikimos", imager.ByteMaker("53.jpg"));


            u4.Businesses.Add(b53);
            b53.Products.Add(p53_0);
            b53.Workdays.Add(t45_0);
            b53.Workdays.Add(t45_1);

            Product p54_0 = CreateProduct("Padangos", 45, "2x komplektas", "Naujos įviarių tipų padangos", imager.ByteMaker("54.jpg"));
            Business b54 = CreateBusiness("Parduodamos naujos įviarių tipų padangos", "25.215422", "54.769088", "+37062339630", "Nemokamas ir greitas montavimas", imager.ByteMaker("54.jpg"));


            u3.Businesses.Add(b54);
            b54.Products.Add(p54_0);
            b54.Workdays.Add(t45_0);
            b54.Workdays.Add(t45_1);
            //--------
            Product p55_0 = CreateProduct("Cepelinai", 3, "vienetas", "Skanūs su kiauliena", null);
            Business b55 = CreateBusiness("Parduodami cepelinai", "25.182170", "54.715578", "+37062449630", "Cepelinai su kiauliena. Pridedame grietinės", imager.ByteMaker("cipakai.jpg"));

            u2.Businesses.Add(b55);
            b55.Products.Add(p55_0);
            b55.Workdays.Add(t45_0);
            b55.Workdays.Add(t45_1);

            Product p56_0 = CreateProduct("Cepelinai", 4, "vienetas", "Su jautiena", null);
            Business b56 = CreateBusiness("Parduodami naminiai cepelinai", "25.232329", "54.721924", "+37062559630", "Su jautiena.", imager.ByteMaker("cipakai.jpg"));


            u1.Businesses.Add(b56);
            b56.Products.Add(p56_0);
            b56.Workdays.Add(t45_0);
            b56.Workdays.Add(t45_1);

            Product p57_0 = CreateProduct("Cepelinai", 4, "vienetas", "Skanus močiutės cepelinai su varške", null);
            Business b57 = CreateBusiness("Parduodami cepelinai su varške", "25.292794", "54.722320", "+37062669630", "Pridedame grietinės", imager.ByteMaker("cipakai.jpg"));


            u9.Businesses.Add(b57);
            b57.Products.Add(p57_0);
            b57.Workdays.Add(t45_0);
            b57.Workdays.Add(t45_1);

            Product p58_0 = CreateProduct("Cepelinai", 9, "2 vienetai", "Dideli cepelinai", null);
            Business b58 = CreateBusiness("Dideli cepelinai", "25.153845", "54.713634", "+37062179630", "Pridedame spirgučius su grietine", imager.ByteMaker("cipakai.jpg"));


            u7.Businesses.Add(b58);
            b58.Products.Add(p58_0);
            b58.Workdays.Add(t45_0);
            b58.Workdays.Add(t45_1);

            Product p59_0 = CreateProduct("Cepelinai", 2, "vienetas", "Užšaldyti -15 laipniu pagal Celsijų cepelinai", null);
            Business b59 = CreateBusiness("Parduodami šaldyti cepelinai", "25.306674", "54.624966", "+37062189630", "Galime vietoje pašildyti", imager.ByteMaker("cipakai.jpg"));


            u8.Businesses.Add(b59);
            b59.Products.Add(p59_0);
            b59.Workdays.Add(t45_0);
            b59.Workdays.Add(t45_1);

            Order o1 = new Order { UserId = u2.UserID, ProductId = p1.ProductID, Address = "V. Kudirkos g.", Amount = 1, Comment = "Išsiderėkit 20% nuolaidą dėl žieminių padangų.", DateAdded = DateTime.Now.AddDays(-10) };
            Order o2 = new Order { UserId = u3.UserID, ProductId = p1.ProductID, Address = "Pamėnkalnio g.", Amount = 1, Comment = "Išsiderėkit parduot už dyką...", DateAdded = DateTime.Now.AddDays(-8) };
            Order o3 = new Order { UserId = u4.UserID, ProductId = p1.ProductID, Address = "A. Smetonos g.", Amount = 1, Comment = "Noriu gauti 2 už vieno kainą.", DateAdded = DateTime.Now.AddDays(-3) };
            Order o4 = new Order { UserId = u5.UserID, ProductId = p6.ProductID, Address = "Žolyno g.", Amount = 10, Comment = "Mano telefono numeris: +37064214253.", DateAdded = DateTime.Now.AddDays(-1) };
            Order o5 = new Order { UserId = u6.UserID, ProductId = p6.ProductID, Address = "Bistryčios g.", Amount = 5, Comment = "Mano paštas: lapinas3456@gmail.com.", DateAdded = DateTime.Now.AddDays(-5) };
            Order o6 = new Order { UserId = u2.UserID, ProductId = p7.ProductID, Address = "Šilo g.", Amount = 4, Comment = "Bėgdamas nuo policijos pradūriau 4. Todėl ir reikia keturių. Mano paštas:testinis1422@gmail.com, mano telefonas: +37064124357.", DateAdded = DateTime.Now.AddDays(-5) };
            Order o7 = new Order { UserId = u3.UserID, ProductId = p11.ProductID, Address = "Įsruties g. 05-13", Amount = 1, Comment = "Mano paštas:testinis1422@gmail.com, mano telefonas: +37064124357.", DateAdded = DateTime.Now.AddDays(-7) };
            Order o8 = new Order { UserId = u4.UserID, ProductId = p11.ProductID, Address = "Kalno g. 13-10", Amount = 3, Comment = "Atvežkit per 3 dienas arba man bus labai pikta.", DateAdded = DateTime.Now.AddDays(-8) };
            Order o9 = new Order { UserId = u5.UserID, ProductId = p11.ProductID, Address = "Kalno g. 02-03", Amount = 5, Comment = "Užsakau visai šeimynai. Nenoriu atskleisti savo pašto. Prašau susisiekit.", DateAdded = DateTime.Now.AddDays(-10) };
            Order o10 = new Order { UserId = u8.UserID, ProductId = p11.ProductID, Address = "Kalno g. 01-07", Amount = 10, Comment = "Pas mus tradicija. Kasmet visas daugiabutis užsisako cepelinų ir kartu su šeima valgo.", DateAdded = DateTime.Now.AddHours(-26) };
            u2.Orders.Add(o1);
            u3.Orders.Add(o2);
            u4.Orders.Add(o3);
            u5.Orders.Add(o4);
            u6.Orders.Add(o5);
            u2.Orders.Add(o6);
            u3.Orders.Add(o7);
            u4.Orders.Add(o8);
            u5.Orders.Add(o9);
            u8.Orders.Add(o10);

            p1.Orders.Add(o1);
            p1.Orders.Add(o2);
            p1.Orders.Add(o3);
            p6.Orders.Add(o4);
            p6.Orders.Add(o5);
            p7.Orders.Add(o6);
            p11.Orders.Add(o7);
            p11.Orders.Add(o8);
            p11.Orders.Add(o9);
            p11.Orders.Add(o10);




            return new User[] { u1, u2, u3, u4, u5, u6, u7, u8, u9, u10, u11 };
        }

    }
}

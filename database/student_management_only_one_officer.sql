SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

CREATE DATABASE IF NOT EXISTS `student_management` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `student_management`;

CREATE TABLE `tblalinanders` (
  `id` int(11) NOT NULL,
  `ders_no` int(11) NOT NULL,
  `ogrenci_no` int(11) NOT NULL,
  `vize_sonuc` int(11) DEFAULT NULL,
  `final_sonuc` int(11) DEFAULT NULL,
  `butunleme_sonuc` int(11) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `modified_at` datetime DEFAULT NULL ON UPDATE current_timestamp(),
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
DELIMITER $$
CREATE TRIGGER `trg_tblalinanders_before_insert` BEFORE INSERT ON `tblalinanders` FOR EACH ROW BEGIN
	IF ((SELECT deleted_at FROM tblkatalogders WHERE ders_no = `NEW`.ders_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu ders veritabanından silinmiş";
	END IF;

	IF EXISTS(SELECT * FROM tblalinanders WHERE ders_no = `NEW`.ders_no AND ogrenci_no = `NEW`.ogrenci_no) THEN
    		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğrenci zaten bu derse kayıtlı";
    	END IF;

	SET `NEW`.created_at = CURRENT_TIMESTAMP;
	SET `NEW`.modified_at = NULL;
	SET `NEW`.deleted_at = NULL;
	SET `NEW`.vize_sonuc = NULL;
	SET `NEW`.final_sonuc = NULL;
	SET `NEW`.butunleme_sonuc = NULL;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_tblalinanders_before_update` BEFORE UPDATE ON `tblalinanders` FOR EACH ROW BEGIN
	IF ((SELECT deleted_at FROM tblkatalogders WHERE ders_no = `NEW`.ders_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu ders veritabanından silinmiş";
	END IF;

	IF ((`NEW`.ders_no != `OLD`.ders_no) OR (`NEW`.ogrenci_no != `OLD`.ogrenci_no)) THEN
		IF EXISTS(SELECT * FROM tblalinanders WHERE ders_no = `NEW`.ders_no AND ogrenci_no = `NEW`.ogrenci_no) THEN
    			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğrenci zaten bu derse kayıtlı";
    		END IF;
	END IF; 

	IF (`NEW`.final_sonuc IS NULL AND (`NEW`.butunleme_sonuc IS NOT NULL )) THEN		
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Final sonucu girilmeden bütünleme sonucu girilemez";		
	END IF;

	IF (`NEW`.vize_sonuc IS NOT NULL AND (`NEW`.vize_sonuc < 0 OR `NEW`.vize_sonuc > 100)) THEN		
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Vize sonucu 0 ile 100 arasında olmalıdır";		
	END IF;

	IF (`NEW`.final_sonuc IS NOT NULL AND (`NEW`.final_sonuc < 0 OR `NEW`.final_sonuc > 100)) THEN		
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Final sonucu 0 ile 100 arasında olmalıdır";		
	END IF;

	IF (`NEW`.butunleme_sonuc IS NOT NULL AND (`NEW`.butunleme_sonuc < 0 OR `NEW`.butunleme_sonuc > 100)) THEN		
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bütünleme sonucu 0 ile 100 arasında olmalıdır";		
	END IF;

	SET `NEW`.created_at = `OLD`.created_at;
	SET `NEW`.modified_at = CURRENT_TIMESTAMP;

	IF (`NEW`.deleted_at IS NOT NULL) THEN
		SET `NEW`.deleted_at = CURRENT_TIMESTAMP;
	END IF;
END
$$
DELIMITER ;

CREATE TABLE `tblbolum` (
  `bolum_no` int(11) NOT NULL,
  `bolum_adi` varchar(200) NOT NULL,
  `donem_sayisi` int(11) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `modified_at` datetime DEFAULT NULL ON UPDATE current_timestamp(),
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
DELIMITER $$
CREATE TRIGGER `trg_tblbolum_before_insert` BEFORE INSERT ON `tblbolum` FOR EACH ROW BEGIN	
	IF (`NEW`.donem_sayisi < 1) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Dönem sayısı 0'dan büyük olmalıdır";
	END IF;

	IF (LENGTH(`NEW`.bolum_adi) < 5) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bölüm adı en az 5 karakter olmalıdır";
	END IF;

	SET `NEW`.created_at = CURRENT_TIMESTAMP;
	SET `NEW`.modified_at = NULL;
	SET `NEW`.deleted_at = NULL;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_tblbolum_before_update` BEFORE UPDATE ON `tblbolum` FOR EACH ROW BEGIN	
	IF (`NEW`.donem_sayisi < 1) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Dönem sayısı 0'dan büyük olmalıdır";
	END IF;

	IF (LENGTH(`NEW`.bolum_adi) < 5) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bölüm adı en az 5 karakter olmalıdır";
	END IF;

	SET `NEW`.created_at = `OLD`.created_at;
	SET `NEW`.modified_at = CURRENT_TIMESTAMP;

	IF (`NEW`.deleted_at IS NOT NULL) THEN
		IF EXISTS(SELECT * FROM tblkatalogders WHERE bolum_no = `NEW`.bolum_no) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu bölüme kayıtlı dersler olduğu için bölümü silemezsiniz";
		END IF;

		IF EXISTS(SELECT * FROM tblogrenci WHERE bolum_no = `NEW`.bolum_no) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu bölüme kayıtlı öğrenciler olduğu için bölümü silemezsiniz";
		END IF;

		IF EXISTS(SELECT * FROM tblogretimuyesi WHERE bolum_no = `NEW`.bolum_no) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu bölüme kayıtlı öğretim görevlileri olduğu için bölümü silemezsiniz";
		END IF;

		SET `NEW`.deleted_at = CURRENT_TIMESTAMP;		
	END IF;
END
$$
DELIMITER ;

CREATE TABLE `tbldanismanonay` (
  `id` int(11) NOT NULL,
  `ogrenci_no` int(11) NOT NULL,
  `katalog_ders_kodu` int(11) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `modified_at` datetime DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
DELIMITER $$
CREATE TRIGGER `trg_tbldanismanonay_before_insert` BEFORE INSERT ON `tbldanismanonay` FOR EACH ROW BEGIN	
	IF ((SELECT deleted_at FROM tblogrenci WHERE ogrenci_no = `NEW`.ogrenci_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu öğrenci veritabanından silinmiş";
	END IF;

	IF ((SELECT deleted_at FROM tblkatalogders WHERE ders_no = `NEW`.katalog_ders_kodu) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu ders veritabanından silinmiş";
	END IF;

	IF EXISTS(SELECT * FROM tbldanismanonay WHERE ogrenci_no = `NEW`.ogrenci_no AND katalog_ders_kodu = `NEW`.katalog_ders_kodu) THEN
    		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğrenci zaten bu ders için danışman onayı bekliyor";
    	END IF;

	IF EXISTS(SELECT * FROM tblalinanders WHERE ogrenci_no = `NEW`.ogrenci_no AND ders_no = `NEW`.katalog_ders_kodu) THEN
    		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğrenci zaten bu derse kayıtlı";
    	END IF;

	SET `NEW`.created_at = CURRENT_TIMESTAMP;
	SET `NEW`.modified_at = NULL;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_tbldanismanonay_before_update` BEFORE UPDATE ON `tbldanismanonay` FOR EACH ROW BEGIN	
	IF ((SELECT deleted_at FROM tblogrenci WHERE ogrenci_no = `NEW`.ogrenci_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu öğrenci veritabanından silinmiş";
	END IF;

	IF ((SELECT deleted_at FROM tblkatalogders WHERE ders_no = `NEW`.katalog_ders_kodu) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu ders veritabanından silinmiş";
	END IF;

	IF ((`NEW`.ogrenci_no != `OLD`.ogrenci_no) OR (`NEW`.katalog_ders_kodu != `OLD`.katalog_ders_kodu)) THEN
		IF EXISTS(SELECT * FROM tbldanismanonay WHERE ogrenci_no = `NEW`.ogrenci_no AND katalog_ders_kodu = `NEW`.katalog_ders_kodu) THEN
    			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğrenci zaten bu ders için danışman onayı bekliyor";
	    	END IF;
	
		IF EXISTS(SELECT * FROM tblalinanders WHERE ogrenci_no = `NEW`.ogrenci_no AND ders_no = `NEW`.katalog_ders_kodu) THEN
    			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğrenci zaten bu derse kayıtlı";
	    	END IF;
	END IF;

	SET `NEW`.created_at = `OLD`.created_at;
	SET `NEW`.modified_at = CURRENT_TIMESTAMP;
END
$$
DELIMITER ;

CREATE TABLE `tblkatalogders` (
  `ders_no` int(11) NOT NULL,
  `bolum_no` int(11) NOT NULL,
  `ogretim_uyesi_no` int(11) NOT NULL,
  `ders_adi` varchar(100) NOT NULL,
  `kredi` int(11) NOT NULL,
  `ders_donemi` int(11) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `modified_at` datetime DEFAULT NULL ON UPDATE current_timestamp(),
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
DELIMITER $$
CREATE TRIGGER `trg_tblkatalogders_before_insert` BEFORE INSERT ON `tblkatalogders` FOR EACH ROW BEGIN	
	IF ((SELECT deleted_at FROM tblbolum WHERE bolum_no = `NEW`.bolum_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu bölüm veritabanından silinmiş";
	END IF;

	IF ((SELECT deleted_at FROM tblogretimuyesi WHERE ogretim_uye_no = `NEW`.ogretim_uyesi_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu öğretim görevlisi veritabanından silinmiş";
	END IF;

	IF (`NEW`.kredi < 1) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Kredi 0'dan büyük olmalıdır";
	END IF;

	IF (LENGTH(`NEW`.ders_adi) < 2) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ders adı en az 2 karakter olmalıdır";
	END IF;

	IF (`NEW`.ders_donemi < 1) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ders dönemi 0'dan büyük olmalıdır";
	END IF;

	IF (`NEW`.ders_donemi > (SELECT donem_sayisi FROM tblbolum WHERE bolum_no = `NEW`.bolum_no)) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ders dönemi, ders bölümünün toplam dönem sayısından büyük olamaz";
	END IF;

	SET `NEW`.created_at = CURRENT_TIMESTAMP;
	SET `NEW`.modified_at = NULL;
	SET `NEW`.deleted_at = NULL;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_tblkatalogders_before_update` BEFORE UPDATE ON `tblkatalogders` FOR EACH ROW BEGIN	
	IF ((SELECT deleted_at FROM tblbolum WHERE bolum_no = `NEW`.bolum_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu bölüm veritabanından silinmiş";
	END IF;

	IF ((SELECT deleted_at FROM tblogretimuyesi WHERE ogretim_uye_no = `NEW`.ogretim_uyesi_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu öğretim görevlisi veritabanından silinmiş";
	END IF;

	IF (`NEW`.kredi < 1) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Kredi 0'dan büyük olmalıdır";
	END IF;

	IF (LENGTH(`NEW`.ders_adi) < 2) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ders adı en az 2 karakter olmalıdır";
	END IF;

	IF (`NEW`.ders_donemi < 1) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ders dönemi 0'dan büyük olmalıdır";
	END IF;

	IF (`NEW`.ders_donemi > (SELECT donem_sayisi FROM tblbolum WHERE bolum_no = `NEW`.bolum_no)) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ders dönemi, ders bölümünün toplam dönem sayısından büyük olamaz";
	END IF;

	SET `NEW`.created_at = `OLD`.created_at;
	SET `NEW`.modified_at = CURRENT_TIMESTAMP;

	IF (`NEW`.deleted_at IS NOT NULL) THEN
		IF EXISTS(SELECT * FROM tblalinanders WHERE ders_no = `NEW`.ders_no) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu derse kayıtlı öğrenciler olduğu için dersi silemezsiniz";
		END IF;

		IF EXISTS(SELECT * FROM tbldanismanonay WHERE katalog_ders_kodu = `NEW`.ders_no) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu dersi danışman onayına göndermiş öğrenciler olduğu için dersi silemezsiniz";
		END IF;

		SET `NEW`.deleted_at = CURRENT_TIMESTAMP;		
	END IF;
	
END
$$
DELIMITER ;

CREATE TABLE `tblmemur` (
  `memur_no` int(11) NOT NULL,
  `email` varchar(50) NOT NULL,
  `sifre` varchar(50) NOT NULL,
  `ad` varchar(50) NOT NULL,
  `soyad` varchar(50) NOT NULL,
  `telefon` varchar(15) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `modified_at` datetime DEFAULT NULL ON UPDATE current_timestamp(),
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `tblmemur` (`memur_no`, `email`, `sifre`, `ad`, `soyad`, `telefon`, `created_at`, `modified_at`, `deleted_at`) VALUES
(1, 'ahmet.kara@test.com', 'ahmet.kara', 'Ahmet', 'Kara', '05432198765', '2022-01-13 22:46:04', NULL, NULL);
DELIMITER $$
CREATE TRIGGER `trg_tblmemur_before_insert` BEFORE INSERT ON `tblmemur` FOR EACH ROW BEGIN

	IF (`NEW`.email NOT LIKE '_%@__%.__%') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz email adresi";    		
    	END IF;

	IF (LENGTH(`NEW`.sifre) < 6) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Sifre 6 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.ad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.soyad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Soyad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.telefon) < 10) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Telefon numarasi 10 karakterden az olamaz";   
	END IF;

	IF (`NEW`.telefon REGEXP '[A-Z]') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz telefon numarasi";    		
    	END IF;

	SET `NEW`.created_at = CURRENT_TIMESTAMP;
	SET `NEW`.modified_at = NULL;
	SET `NEW`.deleted_at = NULL;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_tblmemur_before_update` BEFORE UPDATE ON `tblmemur` FOR EACH ROW BEGIN

	IF (`NEW`.email NOT LIKE '_%@__%.__%') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz email adresi";    		
    	END IF;

	IF (LENGTH(`NEW`.sifre) < 6) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Sifre 6 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.ad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.soyad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Soyad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.telefon) < 10) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Telefon numarasi 10 karakterden az olamaz";   
	END IF;

	IF (`NEW`.telefon REGEXP '[A-Z]') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz telefon numarasi";    		
    	END IF;

	SET `NEW`.created_at = `OLD`.created_at;
	SET `NEW`.modified_at = CURRENT_TIMESTAMP;


	IF (`NEW`.deleted_at IS NOT NULL) THEN
		SET `NEW`.deleted_at = CURRENT_TIMESTAMP;		
	END IF;
END
$$
DELIMITER ;

CREATE TABLE `tblogrenci` (
  `ogrenci_no` int(11) NOT NULL,
  `bolum_no` int(11) NOT NULL,
  `danisman_no` int(11) NOT NULL,
  `email` varchar(50) NOT NULL,
  `sifre` varchar(50) NOT NULL,
  `ad` varchar(50) NOT NULL,
  `soyad` varchar(50) NOT NULL,
  `telefon` varchar(15) NOT NULL,
  `donem` int(11) NOT NULL,
  `kayit_yili` year(4) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `modified_at` datetime DEFAULT NULL ON UPDATE current_timestamp(),
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
DELIMITER $$
CREATE TRIGGER `trg_tblogrenci_before_insert` BEFORE INSERT ON `tblogrenci` FOR EACH ROW BEGIN

	IF ((SELECT deleted_at FROM tblbolum WHERE bolum_no = `NEW`.bolum_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu bölüm veritabanından silinmiş";
	END IF;

	IF ((SELECT deleted_at FROM tblogretimuyesi WHERE ogretim_uye_no = `NEW`.danisman_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu öğretim görevlisi veritabanından silinmiş";
	END IF;

	IF (`NEW`.email NOT LIKE '_%@__%.__%') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz email adresi";    		
    	END IF;

	IF (LENGTH(`NEW`.sifre) < 6) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Sifre 6 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.ad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.soyad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Soyad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.telefon) < 10) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Telefon numarasi 10 karakterden az olamaz";   
	END IF;

	IF (`NEW`.telefon REGEXP '[A-Z]') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz telefon numarasi";    		
    	END IF;

	SET `NEW`.donem = 1;
	SET `NEW`.kayit_yili = YEAR(CURRENT_DATE);
	SET `NEW`.created_at = CURRENT_TIMESTAMP;
	SET `NEW`.modified_at = NULL;
	SET `NEW`.deleted_at = NULL;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_tblogrenci_before_update` BEFORE UPDATE ON `tblogrenci` FOR EACH ROW BEGIN

	IF ((SELECT deleted_at FROM tblbolum WHERE bolum_no = `NEW`.bolum_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu bölüm veritabanından silinmiş";
	END IF;

	IF ((SELECT deleted_at FROM tblogretimuyesi WHERE ogretim_uye_no = `NEW`.danisman_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu öğretim görevlisi veritabanından silinmiş";
	END IF;

	IF (`NEW`.email NOT LIKE '_%@__%.__%') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz email adresi";    		
    	END IF;

	IF (LENGTH(`NEW`.sifre) < 6) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Sifre 6 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.ad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.soyad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Soyad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.telefon) < 10) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Telefon numarasi 10 karakterden az olamaz";   
	END IF;

	IF (`NEW`.telefon REGEXP '[A-Z]') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz telefon numarasi";    		
    	END IF;

	IF (`NEW`.donem != `OLD`.donem) THEN
		IF (`NEW`.donem < 1) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğrenci dönemi 0'dan büyük olmalıdır";
		END IF;

		IF (`NEW`.donem > (SELECT donem_sayisi FROM tblbolum WHERE bolum_no = `NEW`.bolum_no)) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğrenci dönemi, bölümünün toplam dönem sayısından büyük olamaz";
		END IF;
	END IF;

	SET `NEW`.kayit_yili = `OLD`.kayit_yili;
	SET `NEW`.created_at = `OLD`.created_at;
	SET `NEW`.modified_at = CURRENT_TIMESTAMP;
	
	IF (`NEW`.deleted_at IS NOT NULL) THEN
		IF EXISTS(SELECT * FROM tblalinanders WHERE ogrenci_no = `NEW`.ogrenci_no) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğrenci bazı derslere kayıtlı olduğu için öğrenciyi silemezsiniz";
		END IF;

		IF EXISTS(SELECT * FROM tbldanismanonay WHERE ogrenci_no = `NEW`.ogrenci_no) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğrenci bazı derslerde danışman onayı beklediği için öğrenciyi silemezsiniz";
		END IF;

		SET `NEW`.deleted_at = CURRENT_TIMESTAMP;		
	END IF;
END
$$
DELIMITER ;

CREATE TABLE `tblogretimuyesi` (
  `ogretim_uye_no` int(11) NOT NULL,
  `bolum_no` int(11) NOT NULL,
  `email` varchar(50) NOT NULL,
  `sifre` varchar(50) NOT NULL,
  `ad` varchar(50) NOT NULL,
  `soyad` varchar(50) NOT NULL,
  `telefon` varchar(15) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `modified_at` datetime DEFAULT NULL ON UPDATE current_timestamp(),
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
DELIMITER $$
CREATE TRIGGER `trg_tblogretimuyesi_before_insert` BEFORE INSERT ON `tblogretimuyesi` FOR EACH ROW BEGIN

	IF ((SELECT deleted_at FROM tblbolum WHERE bolum_no = `NEW`.bolum_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu bölüm veritabanından silinmiş";
	END IF;

	IF (`NEW`.email NOT LIKE '_%@__%.__%') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz email adresi";    		
    	END IF;

	IF (LENGTH(`NEW`.sifre) < 6) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Sifre 6 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.ad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.soyad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Soyad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.telefon) < 10) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Telefon numarasi 10 karakterden az olamaz";   
	END IF;

	IF (`NEW`.telefon REGEXP '[A-Z]') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz telefon numarasi";    		
    	END IF;

	SET `NEW`.created_at = CURRENT_TIMESTAMP;
	SET `NEW`.modified_at = NULL;
	SET `NEW`.deleted_at = NULL;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_tblogretimuyesi_before_update` BEFORE UPDATE ON `tblogretimuyesi` FOR EACH ROW BEGIN

	IF ((SELECT deleted_at FROM tblbolum WHERE bolum_no = `NEW`.bolum_no) IS NOT NULL) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Bu bölüm veritabanından silinmiş";
	END IF;

	IF (`NEW`.email NOT LIKE '_%@__%.__%') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz email adresi";    		
    	END IF;

	IF (LENGTH(`NEW`.sifre) < 6) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Sifre 6 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.ad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Ad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.soyad) < 3) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Soyad 3 karakterden az olamaz";   
	END IF;

	IF (LENGTH(`NEW`.telefon) < 10) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Telefon numarasi 10 karakterden az olamaz";   
	END IF;

	IF (`NEW`.telefon REGEXP '[A-Z]') THEN
     		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Geçersiz telefon numarasi";    		
    	END IF;

	SET `NEW`.created_at = `OLD`.created_at;
	SET `NEW`.modified_at = CURRENT_TIMESTAMP;
	
	IF (`NEW`.deleted_at IS NOT NULL) THEN
		IF EXISTS(SELECT * FROM tblkatalogders WHERE ogretim_uyesi_no = `NEW`.ogretim_uye_no) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğretim görevlisi bazı derslerin öğretmeni olduğu için öğretim görevlisini silemezsiniz";
		END IF;

		IF EXISTS(SELECT * FROM tblogrenci WHERE danisman_no = `NEW`.ogretim_uye_no) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "Öğretim görevlisi bazı öğrencilerin danışmanı olduğu için öğretim görevlisini silemezsiniz";
		END IF;

		SET `NEW`.deleted_at = CURRENT_TIMESTAMP;		
	END IF;
END
$$
DELIMITER ;


ALTER TABLE `tblalinanders`
  ADD PRIMARY KEY (`id`),
  ADD KEY `ders_no` (`ders_no`,`ogrenci_no`),
  ADD KEY `ogrenci_no` (`ogrenci_no`);

ALTER TABLE `tblbolum`
  ADD PRIMARY KEY (`bolum_no`);

ALTER TABLE `tbldanismanonay`
  ADD PRIMARY KEY (`id`),
  ADD KEY `ogrenci_no` (`ogrenci_no`,`katalog_ders_kodu`),
  ADD KEY `katalog_ders_kodu` (`katalog_ders_kodu`);

ALTER TABLE `tblkatalogders`
  ADD PRIMARY KEY (`ders_no`),
  ADD KEY `bolum_no` (`bolum_no`,`ogretim_uyesi_no`),
  ADD KEY `ogretim_uyesi_no` (`ogretim_uyesi_no`);

ALTER TABLE `tblmemur`
  ADD PRIMARY KEY (`memur_no`);

ALTER TABLE `tblogrenci`
  ADD PRIMARY KEY (`ogrenci_no`),
  ADD KEY `bolum_no` (`bolum_no`,`danisman_no`),
  ADD KEY `danisman_no` (`danisman_no`);

ALTER TABLE `tblogretimuyesi`
  ADD PRIMARY KEY (`ogretim_uye_no`),
  ADD KEY `bolum_no` (`bolum_no`);


ALTER TABLE `tblalinanders`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `tblbolum`
  MODIFY `bolum_no` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `tbldanismanonay`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `tblkatalogders`
  MODIFY `ders_no` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `tblmemur`
  MODIFY `memur_no` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

ALTER TABLE `tblogrenci`
  MODIFY `ogrenci_no` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `tblogretimuyesi`
  MODIFY `ogretim_uye_no` int(11) NOT NULL AUTO_INCREMENT;


ALTER TABLE `tblalinanders`
  ADD CONSTRAINT `tblalinanders_ibfk_1` FOREIGN KEY (`ders_no`) REFERENCES `tblkatalogders` (`ders_no`),
  ADD CONSTRAINT `tblalinanders_ibfk_2` FOREIGN KEY (`ogrenci_no`) REFERENCES `tblogrenci` (`ogrenci_no`);

ALTER TABLE `tbldanismanonay`
  ADD CONSTRAINT `tbldanismanonay_ibfk_1` FOREIGN KEY (`ogrenci_no`) REFERENCES `tblogrenci` (`ogrenci_no`),
  ADD CONSTRAINT `tbldanismanonay_ibfk_2` FOREIGN KEY (`katalog_ders_kodu`) REFERENCES `tblkatalogders` (`ders_no`);

ALTER TABLE `tblkatalogders`
  ADD CONSTRAINT `tblkatalogders_ibfk_1` FOREIGN KEY (`bolum_no`) REFERENCES `tblbolum` (`bolum_no`),
  ADD CONSTRAINT `tblkatalogders_ibfk_2` FOREIGN KEY (`ogretim_uyesi_no`) REFERENCES `tblogretimuyesi` (`ogretim_uye_no`);

ALTER TABLE `tblogrenci`
  ADD CONSTRAINT `tblogrenci_ibfk_1` FOREIGN KEY (`bolum_no`) REFERENCES `tblbolum` (`bolum_no`),
  ADD CONSTRAINT `tblogrenci_ibfk_2` FOREIGN KEY (`danisman_no`) REFERENCES `tblogretimuyesi` (`ogretim_uye_no`);

ALTER TABLE `tblogretimuyesi`
  ADD CONSTRAINT `tblogretimuyesi_ibfk_1` FOREIGN KEY (`bolum_no`) REFERENCES `tblbolum` (`bolum_no`);

DELIMITER $$
CREATE DEFINER=`root`@`localhost` EVENT `timer_tblogrenci_donem_increment` ON SCHEDULE EVERY 1 YEAR STARTS '2022-01-01 03:00:00' ON COMPLETION NOT PRESERVE ENABLE COMMENT 'Tüm öğrencilerin dönemi her yılın başında 1 artacak' DO UPDATE tblogrenci SET donem = donem+1$$

DELIMITER ;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

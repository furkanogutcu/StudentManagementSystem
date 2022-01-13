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

INSERT INTO `tblalinanders` (`id`, `ders_no`, `ogrenci_no`, `vize_sonuc`, `final_sonuc`, `butunleme_sonuc`, `created_at`, `modified_at`, `deleted_at`) VALUES
(1, 1, 1, 60, 80, NULL, '2022-01-13 17:20:35', '2022-01-13 22:11:23', NULL),
(2, 2, 1, 40, 72, NULL, '2022-01-13 17:20:35', '2022-01-13 20:21:36', NULL),
(3, 3, 1, 56, 21, 60, '2022-01-13 17:20:35', '2022-01-13 20:21:24', NULL),
(4, 4, 1, 50, 76, NULL, '2022-01-13 17:20:35', '2022-01-13 20:21:43', NULL),
(5, 5, 1, 45, 59, NULL, '2022-01-13 17:20:35', '2022-01-13 20:21:51', NULL),
(6, 6, 1, 60, 81, NULL, '2022-01-13 17:20:35', '2022-01-13 20:21:59', NULL),
(7, 7, 1, 73, 80, NULL, '2022-01-13 17:22:39', '2022-01-13 20:23:09', NULL),
(8, 8, 1, 25, 36, 65, '2022-01-13 17:22:39', '2022-01-13 20:23:15', NULL),
(9, 9, 1, 80, 53, NULL, '2022-01-13 17:22:39', '2022-01-13 20:23:21', NULL),
(10, 10, 1, 50, 60, NULL, '2022-01-13 17:22:39', '2022-01-13 20:23:25', NULL),
(11, 11, 1, 45, 58, NULL, '2022-01-13 17:22:39', '2022-01-13 20:23:29', NULL),
(12, 12, 1, 65, 52, NULL, '2022-01-13 17:25:43', '2022-01-13 20:26:09', NULL),
(13, 13, 1, 43, 80, NULL, '2022-01-13 17:25:43', '2022-01-13 22:06:01', NULL),
(14, 14, 1, 75, 92, NULL, '2022-01-13 17:25:43', '2022-01-13 20:26:17', NULL),
(15, 15, 1, 72, 75, NULL, '2022-01-13 17:25:43', '2022-01-13 20:26:22', NULL);
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

INSERT INTO `tblbolum` (`bolum_no`, `bolum_adi`, `donem_sayisi`, `created_at`, `modified_at`, `deleted_at`) VALUES
(1, 'Bilgisayar mühendisliği', 8, '2022-01-13 16:44:12', NULL, NULL),
(2, 'İnşaat mühendisliği', 8, '2022-01-13 16:44:28', NULL, NULL),
(3, 'Makine mühendisliği', 8, '2022-01-13 16:58:43', NULL, NULL),
(4, 'Diş hekimliği', 10, '2022-01-13 16:58:59', NULL, NULL),
(5, 'Çevre mühendisliği', 8, '2022-01-13 16:59:19', NULL, NULL);
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

INSERT INTO `tbldanismanonay` (`id`, `ogrenci_no`, `katalog_ders_kodu`, `created_at`, `modified_at`) VALUES
(16, 1, 16, '2022-01-13 17:38:46', NULL),
(17, 1, 17, '2022-01-13 17:38:46', NULL),
(18, 1, 18, '2022-01-13 17:38:46', NULL),
(19, 1, 19, '2022-01-13 17:38:46', NULL),
(20, 1, 20, '2022-01-13 17:38:46', NULL);
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

INSERT INTO `tblkatalogders` (`ders_no`, `bolum_no`, `ogretim_uyesi_no`, `ders_adi`, `kredi`, `ders_donemi`, `created_at`, `modified_at`, `deleted_at`) VALUES
(1, 1, 1, 'Bilgisayar mühendisliğine giriş', 3, 1, '2022-01-13 16:48:58', NULL, NULL),
(2, 1, 1, 'Programlamaya giriş - 1', 3, 1, '2022-01-13 16:49:11', NULL, NULL),
(3, 1, 4, 'Ayrık matematik', 3, 1, '2022-01-13 16:49:28', NULL, NULL),
(4, 1, 5, 'Fizik - 1', 4, 1, '2022-01-13 16:49:40', '2022-01-13 20:17:58', NULL),
(5, 1, 9, 'Matematik - 1', 4, 1, '2022-01-13 16:49:52', '2022-01-13 20:18:01', NULL),
(6, 1, 1, 'Yabancı dil', 4, 1, '2022-01-13 16:50:04', NULL, NULL),
(7, 1, 3, 'Programlamaya giriş - 2', 4, 2, '2022-01-13 16:50:26', NULL, NULL),
(8, 1, 8, 'Elektrik devreleri ve elektronik', 4, 2, '2022-01-13 16:50:47', NULL, NULL),
(9, 1, 9, 'Olasılık ve istatistiğe giriş', 3, 2, '2022-01-13 16:51:07', NULL, NULL),
(10, 1, 1, 'Fizik - 2', 4, 2, '2022-01-13 16:52:25', NULL, NULL),
(11, 1, 4, 'Matematik - 2', 4, 2, '2022-01-13 16:52:32', NULL, NULL),
(12, 1, 5, 'Veri yapıları', 3, 3, '2022-01-13 16:52:48', NULL, NULL),
(13, 1, 1, 'Sayısal devreler ve mantıksal tasarım', 4, 3, '2022-01-13 16:53:02', NULL, NULL),
(14, 1, 9, 'Doğrusal cebir', 3, 3, '2022-01-13 16:53:18', NULL, NULL),
(15, 1, 3, 'Diferansiyel denklemler', 3, 3, '2022-01-13 16:53:30', NULL, NULL),
(16, 1, 5, 'Algoritmalar', 3, 4, '2022-01-13 16:54:09', NULL, NULL),
(17, 1, 8, 'Sayısal çözümleme', 3, 4, '2022-01-13 16:54:24', NULL, NULL),
(18, 1, 3, 'Nesneye yönelik programlama', 4, 4, '2022-01-13 16:54:52', NULL, NULL),
(19, 1, 4, 'Mesleki ingilizce', 3, 4, '2022-01-13 16:55:03', NULL, NULL),
(20, 1, 5, 'İş sağlığı ve güvenliği', 4, 4, '2022-01-13 16:55:20', NULL, NULL),
(21, 1, 3, 'Bilgisayar mimarisi', 3, 5, '2022-01-13 16:55:33', NULL, NULL),
(22, 1, 8, 'Sistem programlama', 3, 5, '2022-01-13 16:56:15', NULL, NULL),
(23, 1, 1, 'Veritabanı yönetim sistemleri', 3, 5, '2022-01-13 16:56:30', NULL, NULL),
(24, 1, 4, 'Girişimcilik ve yenilikcilik', 2, 5, '2022-01-13 16:56:42', NULL, NULL),
(25, 1, 9, 'İşletim sistemleri', 3, 6, '2022-01-13 16:56:56', NULL, NULL),
(26, 1, 8, 'Özdevinirler kuramı', 3, 6, '2022-01-13 16:57:06', NULL, NULL),
(27, 1, 3, 'Sinyaller ve sistemler', 3, 6, '2022-01-13 16:57:18', NULL, NULL),
(28, 1, 1, 'Mikroişlemciler', 3, 6, '2022-01-13 16:57:28', NULL, NULL),
(29, 1, 3, 'Bilgisayar ağları', 3, 7, '2022-01-13 16:57:47', NULL, NULL),
(30, 1, 4, 'Yazılım mühendisliği', 3, 7, '2022-01-13 16:58:03', NULL, NULL),
(31, 1, 5, 'Mesleki uygulama programı', 9, 8, '2022-01-13 16:58:33', NULL, NULL);
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
(1, 'ahmet.kara@test.com', 'ahmet.kara', 'Ahmet', 'Kara', '05432198765', '2022-01-13 16:43:20', NULL, NULL);
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

INSERT INTO `tblogrenci` (`ogrenci_no`, `bolum_no`, `danisman_no`, `email`, `sifre`, `ad`, `soyad`, `telefon`, `donem`, `kayit_yili`, `created_at`, `modified_at`, `deleted_at`) VALUES
(1, 1, 1, 'baris.kaya@test.com', 'baris.kaya', 'Barış', 'Kaya', '05432198765', 4, 2021, '2022-01-13 17:00:40', '2022-01-13 20:37:19', NULL),
(2, 1, 4, 'betül.toprak@test.com', 'betul.toprak', 'Betül', 'Toprak', '05432198765', 1, 2022, '2022-01-13 17:01:05', NULL, NULL);
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

INSERT INTO `tblogretimuyesi` (`ogretim_uye_no`, `bolum_no`, `email`, `sifre`, `ad`, `soyad`, `telefon`, `created_at`, `modified_at`, `deleted_at`) VALUES
(1, 1, 'ahmet.yilmaz@test.com', 'ahmet.yilmaz', 'Ahmet', 'Yılmaz', '05432198765', '2022-01-13 16:45:22', NULL, NULL),
(2, 2, 'suzan.ucar@test.com', 'suzan.ucar', 'Suzan', 'Uçar', '05432198765', '2022-01-13 16:45:39', NULL, NULL),
(3, 1, 'veli.kaya@test.com', 'veli.kaya', 'Veli', 'Kaya', '05432198765', '2022-01-13 16:45:54', NULL, NULL),
(4, 1, 'betul.turk@test.com', 'betul.turk', 'Betül', 'Türk', '05432198765', '2022-01-13 16:46:16', NULL, NULL),
(5, 1, 'arda.yolcu@test.com', 'arda.yolcu', 'Arda', 'Yolcu', '05432198765', '2022-01-13 16:46:33', NULL, NULL),
(6, 2, 'ali.turk@test.com', 'ali.turk', 'Ali', 'Türk', '05432198765', '2022-01-13 16:46:55', NULL, NULL),
(7, 2, 'mehmet.ekin@test.com', 'mehmet.ekin', 'Mehmet', 'Ekin', '05432198765', '2022-01-13 16:47:14', NULL, NULL),
(8, 1, 'ipek.kilic@test.com', 'ipek.kilic', 'İpek', 'Kılıç', '05432198765', '2022-01-13 16:47:25', NULL, NULL),
(9, 1, 'kemal.uslu@test.com', 'kemal.uslu', 'Kemal', 'Uslu', '05432198765', '2022-01-13 16:47:39', NULL, NULL),
(10, 2, 'pelin.han@test.com', 'pelin.han', 'Pelin', 'Han', '05432198765', '2022-01-13 16:48:12', NULL, NULL),
(11, 2, 'mehmet.turk@test.com', 'mehmet.turk', 'Mehmet', 'Türk', '05432198765', '2022-01-13 16:48:33', NULL, NULL);
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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

ALTER TABLE `tblbolum`
  MODIFY `bolum_no` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

ALTER TABLE `tbldanismanonay`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

ALTER TABLE `tblkatalogders`
  MODIFY `ders_no` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=32;

ALTER TABLE `tblmemur`
  MODIFY `memur_no` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

ALTER TABLE `tblogrenci`
  MODIFY `ogrenci_no` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

ALTER TABLE `tblogretimuyesi`
  MODIFY `ogretim_uye_no` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;


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

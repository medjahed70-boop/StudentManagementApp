# دليل التثبيت والتشغيل - برنامج إدارة التلاميذ

## 📋 الخطوة 1️⃣: استنساخ المشروع

```bash
git clone https://github.com/medjahed70-boop/StudentManagementApp.git
cd StudentManagementApp
```

---

## 🗄️ الخطوة 2️⃣: تشغيل قاعدة البيانات

### أ) فتح SQL Server Management Studio
1. اضغط على **Windows + R**
2. اكتب `ssms` واضغط Enter
3. اختر المخدم الخاص بك (عادة `.\SQLEXPRESS`)

### ب) نفّذ ملف قاعدة البيانات
```
1. اضغط Ctrl + O
2. اختر الملف: Database/CompleteSchema.sql
3. اضغط F5 أو Execute
```

### ج) تحقق من النتائج
```sql
-- التحقق من إنشاء القاعدة
USE StudentManagementDB;
GO

-- عرض الجداول
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';
GO

-- عرض البيانات الافتراضية
SELECT * FROM Users;
SELECT * FROM Classes;
SELECT * FROM Students;
GO
```

---

## ⚙️ الخطوة 3️⃣: تحديث الإعدادات

### تعديل App.config

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <!-- 1. تحديث بيانات الاتصال -->
    <add name="DefaultConnection" 
         connectionString="Server=.\SQLEXPRESS;Database=StudentManagementDB;Integrated Security=true;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  
  <appSettings>
    <!-- 2. تحديث بيانات البريد الإلكتروني (إذا كنت تستخدم Gmail) -->
    <add key="EmailSender" value="your-email@gmail.com" />
    <add key="EmailPassword" value="your-app-password" />
    <!-- ملاحظة: استخدم App Password من حسابك على Google -->
    <!-- الخطوات: Google Account → Security → App passwords -->
    
    <!-- 3. معلومات المدرسة -->
    <add key="SchoolName" value="ثانوية سيدي عيساة" />
    <add key="Province" value="الجزائر غرب" />
    <add key="Ministry" value="وزارة التربية الوطنية" />
  </appSettings>
</configuration>
```

### خطوات الحصول على App Password من Gmail:

1. اذهب إلى: https://myaccount.google.com/
2. اختر **Security** (الأمان)
3. قعّل **2-Step Verification** إن لم تكن مفعلة
4. اذهب إلى **App passwords**
5. اختر **Mail** و **Windows Computer**
6. انسخ كلمة المرور التي تظهر
7. استخدمها في `App.config`

---

## 🚀 الخطوة 4️⃣: تشغيل البرنامج

### أ) فتح المشروع في Visual Studio
```
1. افتح Visual Studio
2. اختر File → Open → Project/Solution
3. اختر StudentManagementApp.sln
```

### ب) تثبيت المكتبات المطلوبة
```
أداة → NuGet Package Manager → Package Manager Console

ثم اكتب:
Install-Package iTextSharp
```

### ج) تشغيل البرنامج
```
اضغط F5 أو Debug → Start Debugging
```

### د) بيانات تسجيل الدخول الافتراضية
```
👤 اسم المستخدم: admin
🔐 كلمة المرور: admin123

أو

👤 اسم المستخدم: teacher1
🔐 كلمة المرور: teacher123

أو

👤 اسم المستخدم: secretary
🔐 كلمة المرور: secretary123
```

---

## ✅ التحقق من النجاح

### 1. قاعدة البيانات
```sql
-- التحقق من الجداول
USE StudentManagementDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;

-- التحقق من البيانات
SELECT * FROM Students;
SELECT * FROM Attendance;
SELECT * FROM Notifications;
```

### 2. تشغيل البرنامج
- [ ] يفتح نموذج تسجيل الدخول
- [ ] تسجيل الدخول بنجاح
- [ ] تظهر الواجهة الرئيسية
- [ ] إمكانية إضافة تلميذ جديد
- [ ] إمكانية تسجيل الحضور

---

## 🔧 استكشاف الأخطاء

### خطأ: "Cannot connect to database"
```
✓ تحقق من أن SQL Server يعمل
✓ تحقق من اسم المخدم في App.config
✓ تحقق من اسم قاعدة البيانات
✓ تحقق من تفعيل Windows Authentication
```

### خطأ: "CompleteSchema.sql not found"
```
✓ تأكد من وجود مجلد Database
✓ تأكد من وجود الملف CompleteSchema.sql
✓ تحقق من المسار الكامل
```

### خطأ: "iTextSharp not installed"
```
في Package Manager Console:
Install-Package iTextSharp
```

---

## 📱 المميزات المتاحة

✅ **إدارة التلاميذ**
- إضافة تلميذ جديد
- البحث عن التلاميذ
- تحديث البيانات
- شطب من القوائم

✅ **الحضور والغياب**
- تسجيل يومي
- حساب ساعات الغياب
- إنذارات آلية

✅ **التقارير**
- تقرير يومي
- تقرير شهري
- إحصائيات متقدمة

✅ **الإشعارات**
- بريد إلكتروني للأولياء
- إنذارات حسب الغياب
- تقارير للإدارة

---

## 📞 الدعم والمساعدة

إذا واجهت أي مشاكل:
- تحقق من ملف README_AR.md
- راجع قسم استكشاف الأخطاء
- تواصل مع فريق الدعم

---

**تاريخ آخر تحديث:** 2026-05-24
**الإصدار:** 1.0 Beta

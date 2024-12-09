# **Torpedó Játék**

Ez egy hálózati, kétjátékos torpedó játék, amely C# és WPF segítségével készült. A játékosok TCP-alapú kommunikáción keresztül kapcsolódnak egymáshoz, és céljuk az ellenfél összes hajójának elsüllyesztése.

---

## **Fejlesztői dokumentáció**

### **Követelmények**
A projekt fejlesztéséhez és futtatásához az alábbiak szükségesek:
- **Visual Studio 2022** vagy újabb
- **.NET 6.0 SDK** vagy újabb
- **WPF** fejlesztői környezet
- Stabil hálózati kapcsolat a játékosok közötti kommunikációhoz

### **Funkcionalitások**
1. **Hálózati kapcsolat**:
   - TCP socket kommunikáció kliens-szerver architektúrával
   - Automatikus hibakezelés hálózati megszakadás esetén

2. **Játékmenet**:
   - **10x10-es rács** a hajók elhelyezéséhez
   - Hajók:
     - 1 repülőgép-hordozó (5 egység)
     - 1 csatahajó (4 egység)
     - 2 tengeralattjáró és cirkáló (3 egység)
     - 1 romboló (2 egység)
   - Hajók elhelyezése: Drag-and-drop funkcióval
   - Körönkénti lövöldözés az ellenfél rácsára

3. **Győzelem feltétele**: Az nyer, aki először elsüllyeszti az összes ellenséges hajót.

4. **Hibakezelés**:
   - Hálózati kapcsolat elvesztése esetén automatikus visszacsatlakozási kísérlet

### **A projekt felépítése**
- **Client**: A játékosok interfésze és logikája
- **Server**: A játék logikájának központi kezelése, a két kliens közötti kommunikáció biztosítása

### **Telepítés és futtatás**
1. **Kliens futtatása**:
   - Nyisd meg a projektet Visual Studio-ban
   - Futtasd a projektet `Debug` nélkül

2. **Szerver futtatása**:
   - Nyisd meg a projektet Visual Studio-ban
   - Futtasd a szervert `Ctrl + F5`-tel

3. **Kapcsolódás**:
   - A két kliens adja meg a szerver IP-címét és portját
   - Indítsd el a játékot a kapcsolódás után

---

## **Felhasználói dokumentáció**

### **Játékszabályok**
1. **Hajók elhelyezése**
   - A játék kezdetén helyezd el a hajóidat a rácson. Használhatod az egérrel történő húzás és forgatás funkciókat.
   - Az elhelyezést követően nyomd meg a *Kész* gombot.

2. **Játék menete**
   - Körönként lőhetsz az ellenfél rácsára.
   - Találat esetén a rácson jelölést látsz (pl. piros szín a találat helyén).
   - Ha az ellenfél hajója elsüllyed, értesítést kapsz.

3. **Győzelem**
   - Az nyer, aki elsőként elsüllyeszti az összes ellenfél hajót.

### **Funkciók a játékos számára**
- **Hajó elhelyezése**: Használj drag-and-drop mozdulatokat és forgató gombokat.
- **Lövések**: Kattints az ellenfél rácsának megfelelő cellájára.

---

## **Ismert hibák és fejlesztési lehetőségek**

### **Hibák**
- Lassú hálózat esetén késhetnek az üzenetek.
- A hajók elhelyezése néha nem működik megfelelően, ha a rács szélén vannak.

### **Jövőbeli fejlesztések**
- AI ellenfél hozzáadása
- Többjátékos mód támogatása
- Haladó statisztikák mentése és megjelenítése

---

## **Fejlesztők**
- **Név**: Bánkuti László, Nagy Kristóf Róbert, Szabó Tamás

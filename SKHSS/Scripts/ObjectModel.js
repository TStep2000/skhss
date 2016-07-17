function makeObjectModel(UserRecordID, Username, Password, Email, RoleID, Parent) {
    var obj = new Object();
    Object.defineProperty(obj, "UserRecordID", { value: UserRecordID, enumerable: true });
    if (Username != "") { Object.defineProperty(obj, "Username", { value: Username, enumerable: true }); }
    if (Password != "") { Object.defineProperty(obj, "Password", { value: Password, enumerable: true }); }
    if (Email != "") { Object.defineProperty(obj, "Email", { value: Email, enumerable: true }); }
    if (RoleID != "") { Object.defineProperty(obj, "RoleID", { value: RoleID, enumerable: true }); }
    if (Parent != "") { Object.defineProperty(obj, "Parent", { value: Parent, enumerable: true }); }
    return JSON.stringify(obj);
}
function makeParentObjectModel(FatherName, MotherName, LastName, Address, City, Zipcode, Email, Phone, Children) {
    var obj = new Object();
    if (FatherName != "") { Object.defineProperty(obj, "FatherName", { value: FatherName, enumerable: true }); }
    if (MotherName != "") { Object.defineProperty(obj, "MotherName", { value: MotherName, enumerable: true }); }
    if (LastName != "") { Object.defineProperty(obj, "LastName", { value: LastName, enumerable: true }); }
    if (Address != "") { Object.defineProperty(obj, "Address", { value: Address, enumerable: true }); }
    if (City != "") { Object.defineProperty(obj, "City", { value: City, enumerable: true }); }
    if (Zipcode != "") { Object.defineProperty(obj, "Zipcode", { value: Zipcode, enumerable: true }); }
    if (Email != "") { Object.defineProperty(obj, "Email", { value: Email, enumerable: true }); }
    if (Phone != "") { Object.defineProperty(obj, "Phone", { value: Phone, enumerable: true }); }
    if (Children != "") { Object.defineProperty(obj, "Children", { value: Children, enumerable: true }); }
    return obj;
}
function makeChildObjectModel(ChildRecordID, FirstName, Birthdate, Teammates) {
    var obj = new Object();
    Object.defineProperty(obj, "ChildRecordID", { value: ChildRecordID, enumerable: true });
    if (FirstName != "") { Object.defineProperty(obj, "FirstName", { value: FirstName, enumerable: true }); }
    if (Birthdate != "") { Object.defineProperty(obj, "Birthdate", { value: Birthdate, enumerable: true }); }
    if (Teammates != "") { Object.defineProperty(obj, "Teammates", { value: Teammates, enumerable: true }); }
    return obj;
}
function makeTeammateObjectModel(TeammateRecordID, Year, SeasonID, TeamID, ShirtID, Accepted, PicID) {
    var obj = new Object();
    Object.defineProperty(obj, "TeammateRecordID", { value: TeammateRecordID, enumerable: true });
    if (Year != "") { Object.defineProperty(obj, "Year", { value: Year, enumerable: true }); }
    if (SeasonID != "") { Object.defineProperty(obj, "SeasonID", { value: SeasonID, enumerable: true }); }
    if (TeamID != "") { Object.defineProperty(obj, "TeamID", { value: TeamID, enumerable: true }); }
    if (ShirtID != "") { Object.defineProperty(obj, "ShirtID", { value: ShirtID, enumerable: true }); }
    if (Accepted != "") { Object.defineProperty(obj, "Accepted", { value: Accepted, enumerable: true }); }
    if (PicID != "") { Object.defineProperty(obj, "PicID", { value: PicID, enumerable: true }); }
    return obj;
}
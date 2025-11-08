If due to any unexpected reason, the .env file does not work, you will need to create a new .env file and place the following code in the file.

// connection string to connect to the database

Mongo_ConnectionString = mongodb+srv://KamranAhmed:Ikkomuitpakistan1046@cluster0.9bli6xt.mongodb.net/

//for forget password functionality to work

SMTP_HOST=smtp.gmail.com

SMTP_PORT=587

SMTP_EnableSsl=true

SMTP_UseDefaultCredentials=false

SMTP_EMAIL=kamranahmedi1272@gmail.com

SMTP_PASSWORD=dfip bmmn fjrc elkp

Use the connection string in Mongodb Compass to connect to the database hosted on Atlas.

To login, All the Usernames and Roles can be found in the database, and all the passwords are 1234 except one username that is:
Username: NishchalBhujel
Password: 5678
Role: Service desk employee

For testing services here are some useful users:
Username: KamranAhmed
Password: 1234
Role: Regular employee

Username: EhtishamAhmad
Password: 1234
Role: Service desk employee

Username: MahmoudHashish
Password: 1234
Role: Admin

Username: SophiaMartinez12
Password: 1234
Role: Regular employee

Regular employee can see his/her own incidents and can create new incidents.

To set the deadline and priority of the incident created by the regular employee and to assign the incident to a service desk employee, there is Update incident functionality available for the service desk employee.
Moreover, a service desk employee can also create new incidents, can change the status of the incidents and can soft delete them by changing the status to closed (which would mean closed without resolving).

Only admin can perform CRUD operations on employees and admin can only do that. He/she can not handle the incidents but only manages the employees. If he/she deletes a regular employee, the tickets created by him/her
will stay. We have decided it keep it that way because even if the employee who reported the incident is gone, the problem is still there and is needed to be resolved. So, incidents will stay in the database and service desk employee will be able to see them, assign them and solve them. The reported By field in the front end will be marked as "Unknown" as the regular employee is removed from the database.
If the admin deletes a service desk employee, the assignedto field in the database will be marked as null and in the front-end, the tickets assigned to him will be marked as "Not assigned yet" and will be needed to assign to someone else later on. While deleting the employee, such message will be displayed to infor the admin of the act.

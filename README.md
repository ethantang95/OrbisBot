# OrbisBot
Orbisbot is a currently in development discord chat bot with many features already. It is built in C# on its own custom commands framework over the Discord.NET Core APIs.

### Features

#### Per-User Per-Command Per-Channel Permissions
Orbisbot have a strong permission system for the users in the channel and commands. All users can be assigned a permission role of Restricted-User, User, Moderator, Admin, and Owner. Such roles are also set for permissions and is unique per channel. Permission roles for most commands are also changeable for the channel it is in. All permissions are saved as a file in My Documents folder.

#### Integrated Cooldown Timer
All commands that Orbisbot has an integrated and adjustable cooldown timer that is unique for each channel to help reduce command spam.

#### Programmable Custom Single Reply Commands
Orbisbot can be programmed by the owner and admins of a channel for quick one liner reply commands. The replies can also be programmed to reference what is passed into it as a parameter such as a person on in the channel through the usage of tokens in the reply.

#### Programmable Server Welcome Messages
Owners and admins can change the welcome message Orbisbot will have when a new person joins the server, using the same token system for custom commands.

#### Auto Role Assignment And User Registration
Through owner's control, Orbisbot can help and automatically assign roles to the members in the channel for the bot. Owners of a channel must register itself with the bot for it to recognize it as the owner.

#### Developer/Host Control
Through the settings file, the bot's host can make the bot recognize them as the developer of the bot and access to developer commands and controls.

#### Command Center Room
For developers or other people that want to host their own version of Orbisbot, a dedicated channel can be set aside as the command center which can be used to view the bot's status, see crashes and problems, or see all the private messages sent to the bot by other users.

### Current Host
The current version of OrbisBot is hosted on AWS EC2 and is up 99% of the time.

### Future Plans
* Conversation supports
* Event registration and reminders
* Personal reminders
* Specific game trade markets, group activity building, and character builds
* Sound and music support
* Channel wide events and games where the bot is acting as the host

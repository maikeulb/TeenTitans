# Rick And Morty

API client that consumes Rick and Morty data from the Rick And Morty API and
exposes it as a RESTful API.

Technology
----------
* Suave
* JSONProvider

Endpoints
---------

### Characters
| Method     | URI                              | Action                                      |
|------------|----------------------------------|---------------------------------------------|
| `GET`      | `/api/characters`                | `Retrieve all characters`                   |
| `GET`      | `/api/characters/{id}`           | `Retrieve characters`                       |
| `POST`     | `/api/characters`                | `Add character`                             |
| `PUT`      | `/api/characters/{id}`           | `Update character`                          |
| `DELETE`   | `/api/characters/{id}`           | `Remove character`                          |

### Locations
| Method     | URI                              | Action                                     |
|------------|----------------------------------|--------------------------------------------|
| `GET`      | `/api/locations`                 | `Retrieve all locations`                   |
| `GET`      | `/api/locations/{id}`            | `Retrieve location`                        |
| `POST`     | `/api/locations`                 | `Add location`                             |
| `PUT`      | `/api/locations/{id}`            | `Update location`                          |
| `DELETE`   | `/api/locations/{id}`            | `Remove location`                          |

Sample Usage
---------------

`http get http://localhost:8080/api/characters`
```
[
    {
        "gender": "Male", 
        "id": 1, 
        "name": "Rick Sanchez", 
        "origin": "Earth (C-137)", 
        "species": "Human", 
        "status": "Alive"
    }, 
    {
        "gender": "Male", 
        "id": 2, 
        "name": "Morty Smith", 
        "origin": "Earth (C-137)", 
        "species": "Human", 
        "status": "Alive"
    }, 
....
```
`http get http://localhost:8080/api/locations`
```
    {                                                                                                                                                           [842/9065]
        "dimension": "Dimension C-137", 
        "id": 1, 
        "name": "Earth (C-137)", 
        "residents": [
            "https://rickandmortyapi.com/api/character/38", 
            "https://rickandmortyapi.com/api/character/45", 
...
            "https://rickandmortyapi.com/api/character/356", 
            "https://rickandmortyapi.com/api/character/394"
        ], 
        "type": "Planet"
    }, 
    {
        "dimension": "unknown", 
        "id": 2, 
        "name": "Abadango", 
        "residents": [
            "https://rickandmortyapi.com/api/character/6"
        ], 
        "type": "Cluster"
    }, 
....
```

Run
---

You need Mono, forge, and fake.

```
forge fake run
Go to http://localhost:8080 and visit the above endpoint
```

TODO
----
Dockerfile

RWR.display_name = "SPO-10"

function RWR:init()
    self.ping_elements = {}
    self.lock_elements = {}
    for i=0,3 do
        local pos = (i/2)*math.pi
        local ping = self:create_element("light")
        ping:position(
            math.cos(pos),
            math.sin(pos)
        )
        local lock = self:create_element("light")
        lock:position(
            math.cos(pos+0.1),
            math.sin(pos+0.1)
        )
        table.insert(self.ping_elements, ping)
        table.insert(self.lock_elements, lock)
    end
end

function RWR:contact(con)
    
end
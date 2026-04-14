-- band-based RWR
RWR.display_name = "SkyEye 2000/1"

function RWR:indicator(text)
    local el = self:create_element("lamp_round")
    el.text = text
    return el
end

function RWR:init()
    self.display = self:create_element("display")
    self.r9 = self:indicator("R9")
    self.bolt = self:indicator("9K4")
    self.ai = self:indicator("AI")
    self.aaa = self:indicator("AAA")
    self.contacts = {}
end

function RWR:update()

end

function RWR:contact(con)
    local cinfo = self.self.contacts[con.id]
    if cinfo then
        cinfo.con.lock = con.lock
        cinfo.ttd = game.leveltime()+3
        cinfo.con.pos = con.pos
    end
end